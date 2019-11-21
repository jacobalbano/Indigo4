using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Indigo.Engine.Rendering
{
    public class RenderContext
    {
        public SpriteBatch SpriteBatch { get; internal set; }
        public Texture2D Pixel { get; internal set; }
    }
}
