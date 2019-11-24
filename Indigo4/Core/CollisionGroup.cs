using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo.Core
{
    public class CollisionGroup
    {
        public string Name { get; }

        public CollisionGroup(string name)
        {
            Name = name;
        }
    }
}
