using Indigo.Core.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo
{
    public partial class Space
    {
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

        public CollisionGroup GetCollisionGroup(string name)
        {
            return new CollisionGroup(this, name); // TODO cache
        }

        public virtual void Render()
        {
        }

        public virtual void Update()
        {
            throw new NotImplementedException();
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
