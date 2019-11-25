using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo.Input
{
    public interface IButton
    {
        bool Up { get; }
        bool Down { get; }
        bool Pressed { get; }
        bool Released { get; }
    }
}
