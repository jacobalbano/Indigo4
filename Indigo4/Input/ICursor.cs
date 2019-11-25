using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo.Input
{
    public interface ICursor
    {
        float X { get; }
        float Y { get; }

        float DeltaX { get; }
        float DeltaY { get; }
    }
}
