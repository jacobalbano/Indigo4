using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo.Input
{
    public class Cursor
    {
        public float X { get; private set; }
        public float Y { get; private set; }

        public float DeltaX { get; private set; }
        public float DeltaY { get; private set; }
    }
}
