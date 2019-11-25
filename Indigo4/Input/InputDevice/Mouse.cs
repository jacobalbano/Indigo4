
using System;
using System.Collections.Generic;
using System.Linq;
using Indigo.Core;
using XNA = Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Indigo.Input;
using Indigo.Input.InputDevice;

namespace Indigo.Inputs
{
	public class Mouse : InputDeviceBase<Mouse, Mouse.InputMessage, Mouse.InputDriver>
	{
        public class InputMessage : InputMessageBase
        {
            public XNA.MouseState MouseState;
        }

        public class InputDriver : InputDriverBase { }

        private class Wheel : IWheel
        {
            public bool Moved => Delta != 0;
            public float Delta { get; private set; }

            public Wheel(InputDriver driver)
            {
                driver.Update += Driver_Update;
            }

            private void Driver_Update(InputMessage msg)
            {
                var cwheel = msg.MouseState.ScrollWheelValue;
                var dwheel = cwheel - _lastWheel;
                _lastWheel = cwheel;

                //	eat input if no focus
                if (msg.AcceptNewInputs)
                    Delta = dwheel;
            }

            private int _lastWheel;
        }

        private class Button : IButton
        {
            public bool Up => !Down;
            public bool Down => state == InputState.Down || Pressed;
            public bool Pressed => state == InputState.Pressed;
            public bool Released => state == InputState.Released;

            private readonly ButtonCode code;

            public Button(ButtonCode button, InputDriver driver)
            {
                code = button;
                driver.Update += Driver_Update;
            }

            private void Driver_Update(InputMessage msg)
            {
                if (state == InputState.Pressed) state = InputState.Down;
                if (state == InputState.Released) state = InputState.Up;

                var xnaState = XNA.ButtonState.Released;
                switch (code)
                {
                    case ButtonCode.Left: xnaState = msg.MouseState.LeftButton; break;
                    case ButtonCode.Right: xnaState = msg.MouseState.RightButton; break;
                    case ButtonCode.Middle: xnaState = msg.MouseState.MiddleButton; break;
                    case ButtonCode.X1: xnaState = msg.MouseState.XButton1; break;
                    case ButtonCode.X2: xnaState = msg.MouseState.XButton2; break;
                }

                if (xnaState == XNA.ButtonState.Pressed)
                {
                    if (state == InputState.Up && msg.AcceptNewInputs) state = InputState.Pressed;
                }
                else
                {
                    if (state == InputState.Down) state = InputState.Released;
                }
            }

            private InputState state;
            private enum InputState { Up, Down, Pressed, Released };
        }

        private class Cursor : ICursor
        {
            public float X => _mousePos.X;
            public float Y => _mousePos.Y;
            public float DeltaX => _delta.X;
            public float DeltaY => _delta.Y;

            public Cursor(InputDriver driver)
            {
                driver.Update += Driver_Update;
            }

            private void Driver_Update(InputMessage msg)
            {
                var newPos = new Point(msg.MouseState.X, msg.MouseState.Y);
                if (float.IsNaN(newPos.X)) newPos.X = _mousePos.X;
                if (float.IsNaN(newPos.Y)) newPos.Y = _mousePos.Y;

                _delta = newPos - _mousePos;
                _mousePos = newPos;
            }

            private Point _mousePos, _delta;
        }

		internal Mouse()
		{
			buttons = new Dictionary<ButtonCode, Button>();

            Left = AddButton(ButtonCode.Left);
            Right = AddButton(ButtonCode.Right);
            Middle  = AddButton(ButtonCode.Middle);
            Button1 = AddButton(ButtonCode.X1);
            Button2 = AddButton(ButtonCode.X2);
            Any = new AnyButton(Left, Right, Middle, Button1, Button2);
		}

        private Button AddButton(ButtonCode code)
        {
            var b = new Button(code, Driver);
            buttons[code] = b;
            return b;
        }

        protected override InputMessage GetMessage()
        {
            message.MouseState = XNA.Mouse.GetState();
            return message;
        }

        private readonly InputMessage message = new InputMessage();
		
		#region Buttons
		public IButton Last { get; private set; }
		public readonly IButton Any, Left, Right, Middle, Button1, Button2;
		#endregion
		
        private Dictionary<ButtonCode, Button> buttons;

        private enum ButtonCode { Left, Right, Middle, X1, X2 }
    }
}
