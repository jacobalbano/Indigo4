namespace Indigo
{
    public partial class Space
    {
        public class CollisionGroup
        {
            internal Space Owner { get; }
            public string Name { get; }

            internal CollisionGroup(Space owner, string name)
            {
                Owner = owner;
                Name = name;
            }
        }
    }
}
