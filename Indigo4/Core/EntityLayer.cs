using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo.Core
{
    public class EntityLayer
    {
        public string Name { get; }

        public int Depth { get; set; } = 0;

        public float ScrollX { get; set; } = 1;
        public float ScrollY { get; set; } = 1;

        public EntityLayer(string name)
        {
            Name = name;
        }
    }
}
