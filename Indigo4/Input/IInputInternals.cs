using System;

namespace Indigo.Inputs
{
    internal interface IInputInternals
    {
        void Cycle();
        void SetName(string name);
        void SetState(Button.InputState inputState);
    }
}
