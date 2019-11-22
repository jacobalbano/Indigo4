using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo
{
    public abstract class Component
    {
        public Entity Entity { get; internal set; }

        public bool Active { get; set; }

        public virtual void AddedToEntity() { }
        public virtual void AddedToSpace() { }

        public virtual void Update() { }

        public virtual void RemovedFromEntity() { }
        public virtual void RemovedFromSpace() { }
    }
}
