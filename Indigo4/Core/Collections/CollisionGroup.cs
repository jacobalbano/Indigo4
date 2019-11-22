using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo.Core.Collections
{
    internal class CollisionGroup : Space.ICollisionGroup
    {
        public Space Owner { get; }
        public string Name { get; }

        internal CollisionGroup(Space owner, string name)
        {
            Owner = owner;
            Name = name;
        }
    }
}
