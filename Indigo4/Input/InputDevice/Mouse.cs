
using System;
using System.Collections.Generic;
using System.Linq;
using Indigo.Core;
using XNA = Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Indigo.Inputs
{
	public class Mouse
	{
		public Mouse()
		{
            buttonList = new List<ButtonCode>();
			allButtons = new List<Button>();
			buttons = new Dictionary<ButtonCode, Button>();
            buttonState = new Dictionary<ButtonCode, bool>();

            allButtons.Add(_any = new AnyButton());
            Last = None = new Button();

            Left = AddButton(ButtonCode.Left);
            Right = AddButton(ButtonCode.Right);
            Middle  = AddButton(ButtonCode.Middle);
            Button1 = AddButton(ButtonCode.X1);
            Button2 = AddButton(ButtonCode.X2);

            _mousePos = new Point();
			_delta = new Point();
			_cursorVisible = true;

            var buttonFields = typeof(Mouse).GetFields()
                .Where(field => typeof(Button) == field.FieldType);
			
			foreach (var field in buttonFields)
			{
				var button = (IInputInternals) field.GetValue(this);
				button.SetName(field.Name);
			}
		}
		
		public IEnumerable<Button> Buttons
		{
            get
            {
                for (int i = 0; i < allButtons.Count; i++)
                    if (allButtons[i] != Any) yield return allButtons[i];
            }
		}
		
		internal void Update(bool hasFocus)
		{
            var mouseState = XNA.Mouse.GetState();
            var buttonState = UpdateButtonState(mouseState);
			
			foreach (var b in buttonList)
			{
                var button = buttons[b];
				var internals = button as IInputInternals;
				internals.Cycle();
				
				_any.Update();
				
				if (buttonState[b])
				{
					if (!hasFocus) continue;
					if (button.Up) internals.SetState(Inputs.Button.InputState.Pressed);
					
					_any.OnPress(Last = button);
				}
				else
				{
					if (button.Down) internals.SetState(Inputs.Button.InputState.Released);
				}
			}
			
			var newPos = new Point(mouseState.X, mouseState.Y);
			if (float.IsNaN(newPos.X)) newPos.X = _mousePos.X;
			if (float.IsNaN(newPos.Y)) newPos.Y = _mousePos.Y;
			
			_delta = newPos - _mousePos;
			_mousePos = newPos;

            var cwheel = mouseState.ScrollWheelValue;
			var dwheel = cwheel - _lastWheel;
			_lastWheel = cwheel;
			
			if (hasFocus)	//	eat input if no focus
				WheelDelta = dwheel;
		}

        private Dictionary<ButtonCode, bool> UpdateButtonState(XNA.MouseState state)
        {
            buttonState[ButtonCode.Left] = state.LeftButton == XNA.ButtonState.Pressed;
            buttonState[ButtonCode.Right] = state.RightButton == XNA.ButtonState.Pressed;
            buttonState[ButtonCode.Middle] = state.MiddleButton == XNA.ButtonState.Pressed;
            buttonState[ButtonCode.X1] = state.XButton1== XNA.ButtonState.Pressed;
            buttonState[ButtonCode.X2] = state.XButton2== XNA.ButtonState.Pressed;

            return buttonState;
        }

        private Button AddButton(ButtonCode button)
        {
            var b = new Button();

            buttons[button] = b;
            buttonList.Add(button);
            allButtons.Add(b);

            return b;
        }
		
		private const string ParsePrefix = "Mouse.";
		
		public Button Parse(string name)
		{
			name = name.Trim();
			if (name.StartsWith(ParsePrefix))
				name = name.Substring(ParsePrefix.Length);
			
			return allButtons.FirstOrDefault(key => key.Name == name) ?? None;
		}
		
		#region Cursor
		/// <summary> X position of the mouse on the screen. </summary>
		public int X { get { return (int) _mousePos.X; } }
		
		/// <summary> Y position of the mouse on the screen.</summary>
		public int Y { get { return (int) _mousePos.Y; } }
		
		/// <summary> Difference between the last position of the mouse and its current position. </summary>
		public int DeltaX { get { return (int) _delta.X; } }
		
		/// <summary> Difference between the last position of the mouse and its current position. </summary>
		public int DeltaY { get { return (int) _delta.Y; } }
		#endregion
		
		#region Wheel
		/// <summary>
		/// If the mouse wheel was moved this frame, this was the delta.
		/// </summary>
		public int WheelDelta { get; private set; }
		
		/// <summary>
		/// If the mouse wheel was moved this frame.
		/// </summary>
		public bool WheelMoved { get { return WheelDelta != 0; } }
		#endregion
		
		public class Button : Inputs.Button
		{
			internal Button() {}
			public override string ToString()
			{
				return string.Format("{0}{1}", ParsePrefix, Name);
			}
		}
		
		private class AnyButton : Button
		{
			private Button Actual;

            public override bool Pressed => Actual?.Pressed ?? false;
            public override bool Down => Actual?.Down ?? false;
            public override bool Released => Actual?.Released ?? false;

            public AnyButton()
            {
                Name = "Any";
            }

            public void Update()
			{
				if (Actual != null)
				{
					if (Actual.Up)
						Actual = null;
				}
			}
			
			public void OnPress(Button input)
			{
				if (Actual == null)
					Actual = input;
			}
		}
		
		#region Buttons
		public Button Last { get; private set; }
		public Button None { get; private set; }
		public Button Any { get { return _any; } }
		private AnyButton _any;
		
		public readonly Button Left, Right, Middle, Button1, Button2;
		#endregion
		
		private List<Button> allButtons;
        private List<ButtonCode> buttonList;
        private Dictionary<ButtonCode, Button> buttons;
        private Dictionary<ButtonCode, bool> buttonState;
		private Point _mousePos, _delta;
		private bool _cursorVisible;
		private int _lastWheel;

        private enum ButtonCode
        {
            Left,
            Right,
            Middle,
            X1,
            X2
        }
	}
}
