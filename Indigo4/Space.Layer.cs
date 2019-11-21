namespace Indigo
{
    public partial class Space
    {
        public class Layer
        {
            internal Space Owner { get; }
            public string Name { get; }

            public int Depth { get; set; } = 0;

            public float ScrollX { get; set; } = 1;
            public float ScrollY { get; set; } = 1;

            internal Layer(Space owner, string name)
            {
                Owner = owner;
                Name = name;
            }
        }
    }
}
