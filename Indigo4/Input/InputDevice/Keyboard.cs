using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNA = Microsoft.Xna.Framework.Input;

namespace Indigo.Inputs
{
    public class Keyboard
    {
        internal Keyboard()
        {
            keylist = new List<XNA.Keys>();
            allKeys = new List<Key>();
            keys = new Dictionary<XNA.Keys, Key>();

            _any = new AnyKey();
            Last = None = new Key();
            allKeys.Add(Any);

            A = AddKey(XNA.Keys.A);
            B = AddKey(XNA.Keys.B);
            C = AddKey(XNA.Keys.C);
            D = AddKey(XNA.Keys.D);
            E = AddKey(XNA.Keys.E);
            F = AddKey(XNA.Keys.F);
            G = AddKey(XNA.Keys.G);
            H = AddKey(XNA.Keys.H);
            I = AddKey(XNA.Keys.I);
            J = AddKey(XNA.Keys.J);
            K = AddKey(XNA.Keys.K);
            L = AddKey(XNA.Keys.L);
            M = AddKey(XNA.Keys.M);
            N = AddKey(XNA.Keys.N);
            O = AddKey(XNA.Keys.O);
            P = AddKey(XNA.Keys.P);
            Q = AddKey(XNA.Keys.Q);
            R = AddKey(XNA.Keys.R);
            S = AddKey(XNA.Keys.S);
            T = AddKey(XNA.Keys.T);
            U = AddKey(XNA.Keys.U);
            V = AddKey(XNA.Keys.V);
            W = AddKey(XNA.Keys.W);
            X = AddKey(XNA.Keys.X);
            Y = AddKey(XNA.Keys.Y);
            Z = AddKey(XNA.Keys.Z);

            Num0 = AddKey(XNA.Keys.D0);
            Num1 = AddKey(XNA.Keys.D1);
            Num2 = AddKey(XNA.Keys.D2);
            Num3 = AddKey(XNA.Keys.D3);
            Num4 = AddKey(XNA.Keys.D4);
            Num5 = AddKey(XNA.Keys.D5);
            Num6 = AddKey(XNA.Keys.D6);
            Num7 = AddKey(XNA.Keys.D7);
            Num8 = AddKey(XNA.Keys.D8);
            Num9 = AddKey(XNA.Keys.D9);

            Escape = AddKey(XNA.Keys.Escape);
            LControl = AddKey(XNA.Keys.LeftControl);
            LShift = AddKey(XNA.Keys.LeftShift);
            LAlt = AddKey(XNA.Keys.LeftAlt);
            LSystem = AddKey(XNA.Keys.LeftWindows);
            RControl = AddKey(XNA.Keys.RightControl);
            RShift = AddKey(XNA.Keys.RightShift);
            RAlt = AddKey(XNA.Keys.RightAlt);
            RSystem = AddKey(XNA.Keys.RightWindows);
            Menu = AddKey(XNA.Keys.Apps);
            LBracket = AddKey(XNA.Keys.OemOpenBrackets);
            RBracket = AddKey(XNA.Keys.OemCloseBrackets);
            SemiColon = AddKey(XNA.Keys.OemSemicolon);
            Comma = AddKey(XNA.Keys.OemComma);
            Period = AddKey(XNA.Keys.OemPeriod);
            Quote = AddKey(XNA.Keys.OemQuotes);
            Question = AddKey(XNA.Keys.OemQuestion);
            Pipe = AddKey(XNA.Keys.OemPipe);
            Tilde = AddKey(XNA.Keys.OemTilde);
            Equal = AddKey(XNA.Keys.OemPlus);
            Dash = AddKey(XNA.Keys.OemMinus);
            Space = AddKey(XNA.Keys.Space);
            Return = AddKey(XNA.Keys.Enter);
            BackSpace = AddKey(XNA.Keys.Back);
            Tab = AddKey(XNA.Keys.Tab);

            PageUp = AddKey(XNA.Keys.PageUp);
            PageDown = AddKey(XNA.Keys.PageDown);
            End = AddKey(XNA.Keys.End);
            Home = AddKey(XNA.Keys.Home);
            Insert = AddKey(XNA.Keys.Insert);
            Delete = AddKey(XNA.Keys.Delete);

            KeypadPlus = AddKey(XNA.Keys.OemPlus);
            KeypadMinus = AddKey(XNA.Keys.OemMinus);
            KeypadMultiply = AddKey(XNA.Keys.Multiply);
            KeypadDivide = AddKey(XNA.Keys.Divide);

            Left = AddKey(XNA.Keys.Left);
            Right = AddKey(XNA.Keys.Right);
            Up = AddKey(XNA.Keys.Up);
            Down = AddKey(XNA.Keys.Down);

            Keypad0 = AddKey(XNA.Keys.NumPad0);
            Keypad1 = AddKey(XNA.Keys.NumPad1);
            Keypad2 = AddKey(XNA.Keys.NumPad2);
            Keypad3 = AddKey(XNA.Keys.NumPad3);
            Keypad4 = AddKey(XNA.Keys.NumPad4);
            Keypad5 = AddKey(XNA.Keys.NumPad5);
            Keypad6 = AddKey(XNA.Keys.NumPad6);
            Keypad7 = AddKey(XNA.Keys.NumPad7);
            Keypad8 = AddKey(XNA.Keys.NumPad8);
            Keypad9 = AddKey(XNA.Keys.NumPad9);

            F1 = AddKey(XNA.Keys.F1);
            F2 = AddKey(XNA.Keys.F2);
            F3 = AddKey(XNA.Keys.F3);
            F4 = AddKey(XNA.Keys.F4);
            F5 = AddKey(XNA.Keys.F5);
            F6 = AddKey(XNA.Keys.F6);
            F7 = AddKey(XNA.Keys.F7);
            F8 = AddKey(XNA.Keys.F8);
            F9 = AddKey(XNA.Keys.F9);
            F10 = AddKey(XNA.Keys.F10);
            F11 = AddKey(XNA.Keys.F11);
            F12 = AddKey(XNA.Keys.F12);
            F13 = AddKey(XNA.Keys.F13);
            F14 = AddKey(XNA.Keys.F14);
            F15 = AddKey(XNA.Keys.F15);

            Pause = AddKey(XNA.Keys.Pause);
            Escape = AddKey(XNA.Keys.Escape);

            Control = new InputList.Any(LControl, RControl);
            Shift = new InputList.Any(LShift, RShift);
            Alt = new InputList.Any(RAlt, LAlt);

            var keyFields = typeof(Keyboard).GetFields()
                .Where(field => typeof(Key) == field.FieldType);

            foreach (var field in keyFields)
            {
                var key = (IInputInternals)field.GetValue(this);

                if (key == null)
                    continue;

                key.SetName(field.Name);
            }
        }

        public IEnumerable<Key> Keys()
        {
            for (int i = 0; i < allKeys.Count; i++)
                if (allKeys[i] != Any) yield return allKeys[i];
        }

        public void Update(bool hasFocus)
        {
            var state = XNA.Keyboard.GetState();
            
            foreach (var k in keylist)
            {
                var key = keys[k];
                var internals = key as IInputInternals;
                internals.Cycle();
                _any.Update();

                if (state[k] == XNA.KeyState.Down)
                {
                    if (!hasFocus) continue;
                    if (key.Up) internals.SetState(Button.InputState.Pressed);
                    _any.OnPress(Last = key as Key);
                }
                else
                {
                    if (key.Down) internals.SetState(Button.InputState.Released);
                }
            }
        }

        private Key AddKey(XNA.Keys key)
        {
            var k = new Key();

            keys[key] = k;
            keylist.Add(key);
            allKeys.Add(k);

            return k;
        }
        
        public Key Parse(string name)
        {
            name = name.Trim();
            if (name.StartsWith(ParsePrefix))
                name = name.Substring(ParsePrefix.Length);

            return allKeys.FirstOrDefault(key => key.Name == name) ?? None;
        }
        
        public class Key : Button
        {
            internal Key() { }
            public override string ToString()
            {
                return string.Format("{0}{1}", ParsePrefix, Name);
            }
        }

        private class AnyKey : Key
        {
            public AnyKey()
            {
                Name = "Any";
            }

            private Key Actual;

            public override bool Pressed => Actual?.Pressed ?? false;
            public override bool Down => Actual?.Down ?? false;
            public override bool Released => Actual?.Released ?? false;

            public void Update()
            {
                if (Actual != null)
                {
                    if (Actual.Up)
                        Actual = null;
                }
            }

            public void OnPress(Key input)
            {
                if (Actual == null)
                    Actual = input;
            }
        }

        #region Keys

        /// <summary>
        /// The "Any" key is set when any key on the keyboard is pressed, but only if its state is set to Up.
        /// Once the Any key is pressed, it will not be set again until its state is set to Up again.
        /// </summary>
        public Key Any => _any;
        public Key None { get; private set; }
        public Key Last { get; private set; }

        /// <summary>Alphabet keys.</summary>
        public readonly Key A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z;

        /// <summary>Number keys.</summary>
        public readonly Key Num0, Num1, Num2, Num3, Num4, Num5, Num6, Num7, Num8, Num9;

        ///	<summary>Number pad keys.</summary>
        public readonly Key Keypad0, Keypad1, Keypad2, Keypad3, Keypad4, Keypad5, Keypad6, Keypad7, Keypad8, Keypad9;

        ///	<summary>Modifier keys.</summary>
        public readonly Key LControl, LShift, LAlt, LSystem, RControl, RShift, RAlt, RSystem;

        public readonly InputList.Any Control, Shift, Alt;

        public readonly Key Menu, LBracket, RBracket, SemiColon, Comma, Period, Quote, Question, Pipe, Tilde, Equal, Dash,
            Space, Return, BackSpace, Tab, PageUp, PageDown, End, Home, Insert, Delete, KeypadPlus, KeypadMinus, KeypadMultiply, KeypadDivide, Pause, Escape;

        ///	<summary>Arrow keys.</summary>
        public readonly Key Left, Right, Up, Down;

        ///	<summary>Function keys.</summary>
        public Key F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12, F13, F14, F15;

        #endregion

        private const string ParsePrefix = "Keyboard.";
        private List<XNA.Keys> keylist;
        private List<Key> allKeys;
        private Dictionary<XNA.Keys, Key> keys;
        private AnyKey _any;
    }
}
