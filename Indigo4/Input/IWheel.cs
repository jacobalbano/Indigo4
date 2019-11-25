using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo.Input
{
    public interface IWheel
    {
        bool Moved { get; }
        float Delta { get; }
    }
}
