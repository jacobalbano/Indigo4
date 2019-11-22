using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo.Core.Collections
{
    internal class EntityLayer : Space.IEntityLayer
    {
        public Space Owner { get; }
        public string Name { get; }

        public int Depth { get; set; } = 0;

        public float ScrollX { get; set; } = 1;
        public float ScrollY { get; set; } = 1;

        public EntityLayer(Space owner, string name)
        {
            Owner = owner;
            Name = name;
        }
    }
}
