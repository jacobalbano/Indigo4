using Indigo.Components.Colliders;
using Indigo.Core;
using Indigo.Core.Collections;
using Indigo.Engine;
using Indigo.Engine.Rendering;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Indigo
{
    public partial class Space
    {
        public Camera Camera { get; }

        public EntityList Entities { get; }

        public Space()
        {
            Camera = new Camera();
            Entities = new EntityList(this);
        }

        public virtual void Begin()
        {
        }

        public virtual void Update()
        {
            Entities.Update();
        }

        public virtual void End()
        {
        }
        /*
        public IEntityLayer GetLayer(string name)
        {
            return layers.GetOrAdd(name, () => new EntityLayer(this, name));
        }

        public ICollisionGroup GetCollisionGroup(string name)
        {
            return collisionGroups.GetOrAdd(name, () => new CollisionGroup(this, name));
        }
        */
        internal void Render(Renderer renderer)
        {
            var e = entityLayers
                .OrderByDescending(x => x.Key.Depth)
                .GetEnumerator();

            while (e.MoveNext())
            {
                RenderLayer(renderer, e.Current.Key, e.Current.Value);
                if (e.Current.Key.Depth > 0)
                    break;
            }

            var noLayerEnts = Entities.AsEnumerable().Where(x => x.Layer == null);
            RenderEntities(noLayerEnts, renderer);

            while (e.MoveNext())
                RenderLayer(renderer, e.Current.Key, e.Current.Value);
        }

        private void RenderLayer(Renderer renderer, EntityLayer layer, List<Entity> entities)
        {
            var context = renderer.PrepareContext(Camera, layer);
            RenderEntities(entities, renderer);
            context.End();
        }

        private void RenderEntities(IEnumerable<Entity> entities, Renderer renderer)
        {
            var context = renderer.PrepareContext(Camera);
            foreach (var e in entities)
                e.Components.Render(context);
        }

        internal void SwitchLayer(Entity entity, EntityLayer oldLayer, EntityLayer newLayer)
        {
            if (oldLayer != null && entityLayers.TryGetValue(oldLayer, out var oldList))
                oldList.Remove(entity);

            if (newLayer != null)
                entityLayers.GetOrAdd(newLayer, () => new List<Entity>()).Add(entity);
        }

        private Dictionary<EntityLayer, List<Entity>> entityLayers = new Dictionary<EntityLayer, List<Entity>>();
        private Dictionary<CollisionGroup, List<Collider>> collisionGroups = new Dictionary<CollisionGroup, List<Collider>>();
    }
}
