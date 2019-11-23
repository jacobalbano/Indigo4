using Indigo.Core.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo
{
    public class Entity
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Space Space { get; internal set; }

        public ComponentList Components { get; }

        public Entity()
        {
            Components = new ComponentList(this);
        }

        public virtual void AddedToSpace() { }
        public virtual void RemovedFromSpace() { }

        public virtual void Update()
        {
            Components.Update();
        }
    }
}
