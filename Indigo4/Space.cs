using Indigo.Core.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo
{
    public partial class Space
    {
        public EntityList Entities { get; }

        public Space()
        {
            Entities = new EntityList(this);
        }

        public virtual void Begin()
        {
        }

        public virtual void End()
        {
        }

        public IEntityLayer GetLayer(string name)
        {
            return new EntityLayer(this, name); //TODO cache
        }

        public ICollisionGroup GetCollisionGroup(string name)
        {
            return new CollisionGroup(this, name); // TODO cache
        }

        public virtual void Render()
        {
            //foreach (var e in Entities)
            //    e.Render();
        }

        public virtual void Update()
        {
            Entities.Update();
        }

        public interface ICollisionGroup { string Name { get; } }

        public interface IEntityLayer
        {
            string Name { get; }

            float ScrollX { get; set; }
            float ScrollY { get; set; }

            int Depth { get; set; }
        }
    }
}
