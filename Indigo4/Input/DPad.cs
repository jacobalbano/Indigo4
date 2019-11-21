using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo.Input
{
    public class DPad
    {
        public readonly Button Left, Right, Up, Down;

        public DPad(Button left, Button right, Button up, Button down)
        {
            Left = left;
            Right = right;
            Up = up;
            Down = down;
        }
    }
}
