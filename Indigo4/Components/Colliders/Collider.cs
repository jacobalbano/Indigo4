using System;
using Indigo.Core;
using Indigo.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Indigo.Components.Colliders
{
	public abstract class Collider : Component,
        ICollisionCheck<Collider>
	{
		public abstract float Left { get; }
		public abstract float Right { get; }
		public abstract float Top { get; }
		public abstract float Bottom { get; }

        public abstract bool CollidesWith(float x, float y);
		
		/// <summary>Position of this Collider relative to the Entity it's attached to.</summary>
		public float X
		{
            get => x;
            set => UpdateBoundsIfChanged(value, ref x);
        }
		
		/// <summary>Position of this Collider relative to the Entity it's attached to.</summary>
		public float Y
		{
            get => y;
            set => UpdateBoundsIfChanged(value, ref y);
        }
		
		/// <summary>Origin of this Collider.</summary>
		public float OriginX
		{
            get => ox;
            set => UpdateBoundsIfChanged(value, ref ox);
        }
		
		/// <summary>Origin of this Collider.</summary>
		public float OriginY
		{
            get => oy;
            set => UpdateBoundsIfChanged(value, ref oy);
        }
		
		public string Type
		{
            get => type;
            set => throw new NotImplementedException();// propChanged.Set(this, value, ref type);
        }
		
		public bool CheckBoundingBoxes(Collider other)
		{
			Collider t = this, o = other;
			float tx = t.Left, ty = t.Top, tw = t.Right - t.Left, th = t.Bottom - t.Top;
			float ox = o.Left, oy = o.Top, ow = o.Right - o.Left, oh = o.Bottom - o.Top;
            var distance = MathUtility.DistanceRects(tx, ty, tw, th, ox, oy, ow, oh);
            return distance == 0;
		}
		
		public Collider()
		{
            AssignedParent = ZeroEntity;
            Handlers = new Dictionary<Type, Func<Collider, bool>>();
            var t = GetType();

            var interfaces = t.GetInterfaces()
                .Where(i => i.IsGenericType)
                .Where(i => i.GetGenericTypeDefinition() == typeof(ICollisionCheck<>));

            foreach (var instance in interfaces)
            {
                var interfaceMethod = instance.GetMethods().Single();
                var otherType = instance.GetGenericArguments().Single();

                var map = t.GetInterfaceMap(instance);
                Handlers[otherType] = WrapHandler(map.TargetMethods[0], otherType);
            }
		}

        public bool CollidesWith(Collider other)
        {
            if (!Active)
                return false;

            //  first look for a handler for 'other' on this type
            if (Handlers.TryGetValue(other.GetType(), out var handler))
                return handler(other);

            //  if no local handler found, check if the other type has a handler for this type
            if (other.Handlers.TryGetValue(GetType(), out handler))
                return handler(this);

            //  no handlers found, nothing to do
            return false;
        }

        public override void AddedToEntity()
		{
			AssignedParent = Entity;
            //if (Entity.Space != null)
            //    Entity.Space.Colliders.Add(this);
		}
		
		public override void RemovedFromEntity()
		{
            //if (Entity.Space != null)
            //    Entity.Space.Colliders.Remove(this);

			AssignedParent = ZeroEntity;
		}
		
		protected void UpdateBoundsIfChanged<T>(T value, ref T backingField)
		{
			if (value.Equals(backingField))
				return;
			
			backingField = value;

            //  If we need to do spacial hashing, update buckets here
		}
        
		private float x, y, ox, oy;
		private string type;
		
		internal readonly static Entity ZeroEntity = new Entity();
		protected Entity AssignedParent { get; private set; }
        private Dictionary<Type, Func<Collider, bool>> Handlers;

        #region Really smart stuff
        private Func<Collider, bool> WrapHandler(MethodInfo method, Type otherType)
        {
            return (Func<Collider, bool>) wrapInfo
                .MakeGenericMethod(otherType)
                .Invoke(null, new object[] { method, this });
        }

        [Wrapper]
        private static Func<Collider, bool> WrapHandler<T>(MethodInfo method, object instance) where T : Collider
        {
            var func = (Func<T, bool>)Delegate.CreateDelegate(typeof(Func<T, bool>), instance, method);
            return argument => func((T)argument);
        }

        static Collider()
        {
            wrapInfo = typeof(Collider)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                .Single(m => m.GetCustomAttribute<Wrapper>() != null);
        }

        private static MethodInfo wrapInfo;
        private class Wrapper : Attribute { }

        #endregion
    }
}
