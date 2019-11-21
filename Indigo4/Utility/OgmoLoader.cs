
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using Indigo.Content;
using Indigo.Core;
using Indigo.Utility;
using Indigo.Components.Colliders;
using Indigo.Components.Graphics;

namespace Indigo.Loaders
{
	[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
	public class OgmoConstructorAttribute : Attribute
	{
		public string[] ParameterNames;
		
		/// <summary>
		/// Enables the OgmoLoader class to pass parameters to the constructor of an Entity class.
		/// </summary>
		/// <param name="parameterNames">
		/// <para>A series of parameters naming the Ogmo entity attributes that should be passed to this constructor.</para>
		/// <para>They must appear in the same order as they appear in the constructor of the Entity!</para>
		/// </param>
		public OgmoConstructorAttribute(params string[] parameterNames)
		{
			ParameterNames = parameterNames;
		}
	}
	
	/// <summary>
	/// Entities implementing this interface will be passed their source XML node when constructed.
	/// </summary>
	public interface IOgmoNodeHandler
	{
		void NodeHandler(XmlNode entity);
	}
	
	/// <summary>
	/// Loads an Ogmo Editor level file (.oel) and creates an array of entities to add to a world.
	/// </summary>
	public class OgmoLoader
	{
		public OgmoLoader()
		{
			_types = new Dictionary<string, Type>();
			_gridTypes = new Dictionary<string, GridDefinition>();
			_tilemapTypes = new Dictionary<string, TilemapDefinition>();
		}
		
		public void DefineTilemap(string name, int tileWidth, int tileHeight, Texture texture)
		{
			DefineTilemap(null, name, tileWidth, tileHeight, texture);
		}
		
		public void DefineTilemap(Type entityType, string name, int tileWidth, int tileHeight, Texture texture)
		{
			_tilemapTypes.Add(name, new TilemapDefinition(entityType, tileWidth, tileHeight, texture));
		}
		
		public void DefineGrid(string name, string collisionType, int cellWidth, int cellHeight)
		{
			_gridTypes.Add(name, new GridDefinition(null, cellWidth, cellHeight, collisionType));
		}
		
		public void DefineGrid(Type entityType, string name, string collisionType, int cellWidth, int cellHeight)
		{
			_gridTypes.Add(name, new GridDefinition(entityType, cellWidth, cellHeight, collisionType));
		}
		
		/// <summary>
		/// Register an Entity class under a different name than it appears in code.
		/// This is only needed if you're using a single class to represent multiple Ogmo types,
		/// or in the case of a naming mismatch.
		/// </summary>
		/// <param name="name">The name to register the alias with.</param>
		public void RegisterClassAlias<T>(string name) where T : Entity
		{
			RegisterClassAlias(typeof(T), name);
		}
		
		/// <summary>
		/// Register an Entity class under a different name than it appears in code.
		/// This is only needed if you're using a single class to represent multiple Ogmo types,
		/// or in the case of a naming mismatch.
		/// </summary>
		/// <param name="type">The Type to register.</param>
		/// <param name="name">The name to register the alias with.</param>
		public void RegisterClassAlias(Type type, string name)
		{
			_types[name] = type;
		}
		
		/// <summary>
		/// Get a dictionary of level properties and their values.
		/// </summary>
		/// <param name="oel">The Ogmo level XML.</param>
		/// <returns>The loaded dictionary.</returns>
		public Dictionary<string, string> GetLevelProperties(XmlDocument oel)
		{
			var root = oel.FirstChild;
			var result = new Dictionary<string, string>();
			foreach (XmlAttribute attr in root.Attributes)
				result.Add(attr.Name, attr.Value);
			
			return result;
		}
		
		/// <summary>
		/// Create an object and assign its members from values on an Ogmo level file.
		/// </summary>
		/// <param name="oel">The Ogmo level XML.</param>
		/// <returns>The newly created object.</returns>
		public T GetLevelProperties<T>(XmlDocument oel) where T : new()
		{
			object result = new T();
			ApplyLevelProperties(oel, result);
			return (T) result;
		}
		
		/// <summary>
		/// Assign properties or fields on an object from values on an Ogmo level file.
		/// </summary>
		/// <param name="oel">The Ogmo level XML.</param>
		/// <param name="target">The object to apply properties on.</param>
		public void ApplyLevelProperties<T>(XmlDocument oel, T target) where T : class
		{
			SetProperties(target, oel.FirstChild);
		}
		
		/// <summary>
		/// Attempt to read a single property from a level file, converting it to a specified type.
		/// </summary>
		/// <param name="oel">The Ogmo level XML.</param>
		/// <param name="name">The name of the property to read.</param>
		/// <param name="result">Assigned on success.</param>
		/// <returns>Whether the property was found and read successfully.</returns>
		public bool TryGetLevelProperty<T>(XmlDocument oel, string name, out T result)
		{
			result = default(T);
			var attr = oel.FirstChild.Attributes[name];
			if (attr == null)
				return false;
			
			var parse = ParseValue(oel, name, typeof(T));
			if (parse != null)
				result = (T) parse;
			
			return parse != null;
		}
		
		/// <summary>
		/// Load Entities from an Ogmo level file into an array.
		/// </summary>
		/// <param name="oel">The xml of the level.</param>
		/// <param name="autoLayer">If each entity's layer should be set based on its order in the oel. Only values of 0 will be overwritten.</param>
		/// <returns>An array containing all Entities loaded.</returns>
		public Entity[] BuildLevelAsArray(XmlDocument oel, bool autoLayer = true)
		{
			XmlElement level = oel["level"];
			
			var result = new List<Entity>();
			var layerNodes = new List<XmlNode>();
			var layerCount = 0;
			
			Action<Entity> Add = delegate(Entity e) {
				if (e == null) return;
				if (autoLayer && e.RenderStep == 0)
					e.RenderStep = layerCount;
				
				result.Add(e);
			};
			
			foreach (XmlNode layerNode in level.ChildNodes)
				layerNodes.Add(layerNode);
			
			layerNodes.Reverse();
			
			foreach (var layer in layerNodes)
			{
				++layerCount;
				
				if (layer.Attributes["tileset"] != null)
				{
					Add(OperateOnTilemapLayer(level, layer));
				}
				else if (layer.Attributes["exportMode"] != null)
				{
					Add(OperateOnGridLayer(level, layer));
				}
				else
				{
					foreach (XmlNode entity in layer)
					{
                        if (!_types.TryGetValue(entity.Name, out Type type))
                        {
                            type = TypeUtility.GetTypeFromAllAssemblies<Entity>(entity.Name);
                            if (type != null)
                                _types.Add(entity.Name, type);
                        }

                        if (type == null)
							continue;
						
						var e = CreateInstance(type, entity);
						SetProperties(e, entity);

                        if (e is IOgmoNodeHandler handler)
                            handler.NodeHandler(entity);

                        Add(e);
					}
				}
			}
			
			return result.ToArray();
		}
		
		private Entity OperateOnGridLayer(XmlElement level, XmlNode layer)
		{
            if (!_gridTypes.TryGetValue(layer.Name, out var def))
                return null;

            var width = int.Parse(level.Attributes["width"].Value);
			var height = int.Parse(level.Attributes["height"].Value);
			var tileWidth = def.CellWidth;
			var tileHeight = def.CellHeight;
			
			width -= width % tileWidth;
			height -= height % tileHeight;

            var grid = new HitGrid(width, height, tileWidth, tileHeight) { Type = def.CollisionType };
            var mode = layer.Attributes["exportMode"].Value;
			if (mode.Contains("Bitstring"))
			{
				grid.LoadFromString(layer.InnerText, "", "\n");
			}
			else if (mode.Contains("Rectangles"))
			{
				foreach (XmlNode rect in layer)
				{
					var x = int.Parse(rect.Attributes["x"].Value);
					var y = int.Parse(rect.Attributes["y"].Value);
					var w = int.Parse(rect.Attributes["w"].Value);
					var h = int.Parse(rect.Attributes["h"].Value);

                    if (mode == "Rectangles")
                    {
                        x %= grid.CellWidth;
                        y %= grid.CellHeight;
                        w %= grid.CellWidth;
                        h %= grid.CellHeight;
                    }

                    grid.SetRect(x, y, w, h, true);
				}
			}
			
			Entity e;
			if (def.EntityType == null)
			{
				e = new Entity();
			}
			else
			{
				e = CreateInstance(def.EntityType, layer);
			}
			
			e.Components.Add(grid);
			e.Name = layer.Name;
			e.Components.AddAndRemove(true);

            if (e is IOgmoNodeHandler handler)
                handler.NodeHandler(layer);

            return e;
		}
		
		private Entity OperateOnTilemapLayer(XmlElement level, XmlNode layer)
		{
            if (!_tilemapTypes.TryGetValue(layer.Attributes["tileset"].Value, out var def))
                return null;

            var width = int.Parse(level.Attributes["width"].Value);
			var height = int.Parse(level.Attributes["height"].Value);
			var tileWidth = def.TileWidth;
			var tileHeight = def.TileHeight;
			
			var tilemap = new Tilemap(def.Texture, width, height, tileWidth, tileHeight);
			
			var mode = layer.Attributes["exportMode"].Value;
			if (mode.Contains("CSV"))
			{
				tilemap.LoadFromString(layer.InnerText, ",", "\n");
			}
			else if (mode.Contains("XML"))
			{
				if (mode == "XMLCoords")
				{
					foreach (XmlNode node in layer)
					{
						var tx = int.Parse(node.Attributes["tx"].Value);
						var ty = int.Parse(node.Attributes["ty"].Value);
						var x = int.Parse(node.Attributes["x"].Value);
						var y = int.Parse(node.Attributes["y"].Value);
						
						var id = tilemap.GetIndex(tx, ty);
						tilemap.SetTile(x, y, id);
					}
				}
				else
				{	
					foreach (XmlNode node in layer)
					{
						var x = int.Parse(node.Attributes["x"].Value);
						var y = int.Parse(node.Attributes["y"].Value);
						var id = int.Parse(node.Attributes["id"].Value);
						tilemap.SetTile(x, y, id);
					}
				}
			}
			
			Entity e;
			if (def.EntityType == null)
			{
				e = new Entity();
			}
			else
			{
				e = CreateInstance(def.EntityType, layer);
			}
			
			e.Components.Add(tilemap);
			e.Name = layer.Name;
			e.Components.AddAndRemove(true);

            if (e is IOgmoNodeHandler handler)
                handler.NodeHandler(layer);

            return e;
		}
		
		Entity CreateInstance(Type type, XmlNode entity)
		{
			Entity result = null;
            ConstructorInfo ctor = GetOgmoConstructor(type, out var attribute);
            ctor = ctor ?? type.GetConstructor(flags, null, CallingConventions.Any, Type.EmptyTypes, null);
			
			if (attribute == null)
			{
				if (ctor != null)
				{
					result = (Entity) ctor.Invoke(null);
				}
				else
				{
					throw new Exception("Entity constructor may not take parameters without the use of OgmoConstructor.");
				}
			}
			else
			{
				var paramInfo = ctor.GetParameters();
				var tParams = attribute.ParameterNames;
				var args = new object[tParams.Length];
				
				for (int i = 0; i < args.Length; ++i)
					args[i] = ParseValue(entity, tParams[i], paramInfo[i].ParameterType);
				
				try
				{
					result = (Entity) ctor.Invoke(args);
				}
				catch (Exception ex)
				{
					throw new Exception(string.Format("Error while constructing {0} with OgmoConstructor.", type.Name), ex);
				}
			}
			
			return result;
		}
		
		ConstructorInfo GetOgmoConstructor(Type type, out OgmoConstructorAttribute attribute)
		{
			var constructors = type.GetConstructors(flags);
			var attrType = typeof(OgmoConstructorAttribute);
			foreach (var constructor in constructors)
			{
				var attributes = constructor.GetCustomAttributes(attrType, true);
				if (attributes.Length == 0)
					continue;
				
				attribute = attributes[0] as OgmoConstructorAttribute;
				return constructor;
			}
			
			attribute = null;
			return null;
		}
		
		void SetProperties(object result, XmlNode entity)
		{
			var type = result.GetType();
			foreach (XmlAttribute element in entity.Attributes)
			{
				var name = element.Name;
				var value = element.Value;
				
				var field = type.GetField(name, flags);
				var prop = type.GetProperty(name, flags);
				
				if (prop == null && field == null)
					continue;
				
				Type propType = prop == null ? field.FieldType : prop.PropertyType;
				var toSet = ParseValue(entity, name, propType);
				
				if (prop != null)
					prop.SetValue(result, toSet, null);
				else if (field != null)
					field.SetValue(result, toSet);
			}
		}

		object ParseValue(XmlNode entity, string name, Type valueType)
		{
			var value = entity.Attributes[name].Value;
			if (value == null)
				return null;
			
			var converter = TypeDescriptor.GetConverter(valueType);
			return converter.ConvertFromInvariantString(value);
		}
		
		const BindingFlags flags = BindingFlags.IgnoreCase
				| BindingFlags.Public
				| BindingFlags.NonPublic
				| BindingFlags.Instance
				| BindingFlags.Default;
			
		
		private Dictionary<string, Type> _types;
		private Dictionary<string, GridDefinition> _gridTypes;
		private Dictionary<string, TilemapDefinition> _tilemapTypes;
		
		#region Helper structs
		
		struct GridDefinition
		{
			public GridDefinition(Type entityType, int w, int h, string collisionType)
			{
				EntityType = entityType;
				CellWidth = w;
				CellHeight = h;
				CollisionType = collisionType;
			}
			
			public Type EntityType;
			public int CellWidth, CellHeight;
			public string CollisionType;
		}
		
		struct TilemapDefinition
		{
			public TilemapDefinition(Type entityType, int w, int h, Texture texture)
			{
				EntityType = entityType;
				TileWidth = w;
				TileHeight = h;
				Texture = texture;
			}
			
			public Type EntityType;
			public int TileWidth, TileHeight;
			public Texture Texture;
		}
		
		#endregion
	}
}
