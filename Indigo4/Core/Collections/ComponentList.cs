using System;
using System.Collections.Generic;
using System.Text;
using Indigo.Engine;
using Indigo.Engine.Rendering;

namespace Indigo.Core.Collections
{
    public class ComponentList
    {
        internal ComponentList(Entity owner)
        {
            Owner = owner;
        }

        public TComponent Add<TComponent>(TComponent component) where TComponent : Component
        {
            Contract.ValidateCanAdd(component, Owner);
            components.Add(component);
            return component;
        }

        public bool Remove(Component component)
        {
            Contract.ValidateCanRemove(component, Owner);
            return components.Remove(component);
        }

        internal void Update()
        {
            foreach (var delta in components.SyncYieldingDeltas())
            {
                var c = delta.Item;
                switch (delta.Operation)
                {
                    case BufferedCollection<Component>.DeltaOperation.Add:
                        Contract.ValidateCanAdd(c, Owner);
                        c.Entity = Owner;
                        c.AddedToEntity();
                        break;
                    case BufferedCollection<Component>.DeltaOperation.Remove:
                        Contract.ValidateCanRemove(c, Owner);
                        c.RemovedFromEntity();
                        c.Entity = null;
                        break;
                }
            }

            foreach (var c in components)
                c.Update();
        }

        internal void Render(RenderContext ctx)
        {
            foreach (var c in components)
                c.Render(ctx);
        }

        private BufferedCollection<Component> components = new BufferedCollection<Component>();

        internal Entity Owner { get; }

        private static class Contract
        {
            public static void ValidateCanAdd(Component component, Entity entity)
            {
                if (component.Entity != null)
                    throw new Exception("Tried to add a component to multiple entities at once");
            }

            internal static void ValidateCanRemove(Component component, Entity entity)
            {
                if (component.Entity != entity)
                    throw new Exception("Tried to remove a component from an entity it did not belong to");
            }
        }
    }
}
