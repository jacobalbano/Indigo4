using Indigo.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using XNA = Microsoft.Xna.Framework;
using Indigo.Engine;
using Indigo.Utility;
using Indigo.Engine.Rendering;

namespace Indigo.Components.Graphics
{
    public class Rectangle : Graphic
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Color Color { get; set; }

        protected override void OnRender(RenderContext ctx)
        {
            ctx.SpriteBatch.Draw(ctx.Pixel, Vector2.Zero, new XNA.Rectangle(0, 0, Width, Height), Color);
        }
    }
}
