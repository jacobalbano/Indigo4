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

        public Layer GetLayer(string name)
        {
            return new Layer(this, name); //TODO cache
        }

        public CollisionGroup GetCollisionGroup(string name)
        {
            return new CollisionGroup(this, name);// TODO cache
        }

        public virtual void Render()
        {
        }

        public virtual void Update()
        {
            throw new NotImplementedException();
        }
    }
}
