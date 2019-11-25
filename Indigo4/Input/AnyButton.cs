using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Indigo.Input
{
    public class AnyButton : IButton
    {
        public bool Up => Current == null;
        public bool Down => Current?.Down ?? false;
        public bool Pressed => Current?.Pressed ?? false;
        public bool Released => Current?.Released ?? false;

        public AnyButton(params IButton[] buttons) : this(buttons.AsEnumerable())
        {
        }

        public AnyButton(IEnumerable<IButton> buttons)
        {
            Buttons = buttons.ToList();
        }

        private IButton Current
        {
            get
            {
                if (current?.Up == false)
                    return current;

                var old = current;
                current = Buttons.FirstOrDefault(x => !x.Up);
                return old;
            }
        }

        private readonly IReadOnlyList<IButton> Buttons;
        private IButton current;

        //private class ButtonImpl : IButton
        //{

        //}
    }
}
