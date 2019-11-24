using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Indigo.Engine;
using XNA = Microsoft.Xna.Framework;
using Indigo.Content;
using FNT;
using Indigo.Engine.Rendering;
using Microsoft.Xna.Framework.Graphics;

namespace Indigo.Components.Graphics
{
    public class Text : Graphic
    {
        public FontLibrary.IFont Font
        {
            get { return font; }
            set
            {
                font = value ?? throw new ArgumentNullException(nameof(value));
                dirty = true;
            }
        }

        public string String
        {
            get { return str; }
            set { str = value; dirty = true; }
        }

        public XNA.Rectangle Bounds
        {
            get { Refresh(); return bounds; }
        }

        public Text(FontLibrary.IFont font)
        {
            Font = font ?? throw new ArgumentNullException(nameof(font));
        }

        private void Refresh()
        {
            if (!dirty || str == null)
                return;

            cache = Font.MakeText(str);
            bounds = new XNA.Rectangle { Height = (int) cache.Height, Width = (int) cache.Width };
        }

        protected override void OnRender(RenderContext ctx)
        {
            Refresh();
            cache.Draw(ctx.SpriteBatch, XNA.Vector2.Zero, XNA.Color.White);
        }

        private bool dirty = false;
        private string str;
        private FontLibrary.IFont font;
        private XNA.Rectangle bounds;
        private FontLibrary.IText cache;

    }
}
