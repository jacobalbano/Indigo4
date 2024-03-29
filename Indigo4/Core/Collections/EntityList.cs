﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Indigo.Engine;
using Indigo.Engine.Rendering;

namespace Indigo.Core.Collections
{
    public class EntityList
    {
        internal EntityList(Space owner)
        {
            Space = owner;
        }

        public TEntity Add<TEntity>(TEntity entity) where TEntity : Entity
        {
            Contract.ValidateCanAdd(entity, Space);
            entities.Add(entity);
            return entity;
        }

        public bool Remove(Entity entity)
        {
            Contract.ValidateCanRemove(entity, Space);
            return entities.Remove(entity);
        }

        internal void Update()
        {
            foreach (var delta in entities.SyncYieldingDeltas())
            {
                var e = delta.Item;
                switch (delta.Operation)
                {
                    case BufferedCollection<Entity>.DeltaOperation.Add:
                        Contract.ValidateCanAdd(e, Space);
                        e.Space = Space;
                        e.AddedToSpace();
                        break;
                    case BufferedCollection<Entity>.DeltaOperation.Remove:
                        Contract.ValidateCanRemove(e, Space);
                        e.RemovedFromSpace();
                        e.Space = null;
                        break;
                }
            }

            foreach (var e in entities)
                e.Update();
        }

        private BufferedCollection<Entity> entities = new BufferedCollection<Entity>();
        internal IEnumerable<Entity> AsEnumerable() => entities;

        internal Space Space { get; }

        private static class Contract
        {
            public static void ValidateCanAdd(Entity entity, Space space)
            {
                if (entity.Space != null)
                    throw new Exception("Tried to add an entity to multiple spaces at once");
            }

            internal static void ValidateCanRemove(Entity entity, Space space)
            {
                if (entity.Space != space)
                    throw new Exception("Tried to remove an entity from a space it did not belong to");
            }
        }
    }
}
