using Indigo.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Indigo.Engine;
using Microsoft.Xna.Framework;
using Indigo.Engine.Rendering;
using Indigo.Utility;
using XNA = Microsoft.Xna.Framework.Graphics;

namespace Indigo.Components.Graphics
{
    public class Image : Graphic
    {
        public Texture Texture { get; }

        public bool FlipX
        {
            get => flipX;
            set => Effects = (flipX = value) ? (Effects | XNA.SpriteEffects.FlipHorizontally) : (Effects & ~XNA.SpriteEffects.FlipHorizontally);
        }

        public bool FlipY
        {
            get => flipY;
            set => Effects = (flipY = value) ? (Effects | XNA.SpriteEffects.FlipVertically) : (Effects & ~XNA.SpriteEffects.FlipVertically);
        }

        public void CenterOrigin()
        {
            OriginX = Texture.Width * 0.5f;
            OriginY = Texture.Height * 0.5f;
        }

        public Image(Texture texture)
        {
            Texture = texture;
        }

        protected override void OnRender(RenderContext ctx)
        {
            ctx.SpriteBatch.Draw(Texture.XnaTexture, Vector2.Zero, Color.White);
        }

        private bool flipX, flipY;
        private XNA.SpriteEffects Effects;
    }
}
