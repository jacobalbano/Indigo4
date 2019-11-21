using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo.Content
{
    public class Texture
    {
        internal Texture() { }

        public int Width => XnaTexture.Width;
        public int Height => XnaTexture.Height;

        public Texture2D XnaTexture { get; internal set; }
    }
}
