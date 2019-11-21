using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indigo.Input
{
    public class Button : IInputInternals
    {
        public enum InputState
        {
            Pressed,
            Down,
            Released,
            Up
        }

        public bool Up { get { return !Down; } }
        public virtual bool Down { get { return State == InputState.Down || Pressed; } }
        public virtual bool Pressed { get { return State == InputState.Pressed; } }
        public virtual bool Released { get { return State == InputState.Released; } }

        protected InputState State;

        public string Name { get; protected set; }

        protected Button()
        {
            State = InputState.Up;
        }

        void IInputInternals.SetName(string name)
        {
            Name = name;
        }

        void IInputInternals.SetState(InputState state)
        {
            State = state;
        }

        void IInputInternals.Cycle()
        {
            if (Pressed) State = InputState.Down;
            if (Released) State = InputState.Up;
        }
    }
}
