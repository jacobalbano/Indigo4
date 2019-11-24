using Indigo.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Indigo.Engine;
using Microsoft.Xna.Framework;
using XNA = Microsoft.Xna.Framework;
using Indigo.Engine.Rendering;

namespace Indigo.Components.Graphics
{
    /// <summary>
    /// <para>A background texture that can be repeated horizontally and vertically when drawn.</para>
    /// <para>Really useful for parallax backgrounds, textures, etc.</para>
    /// </summary>
    public class Backdrop : Graphic
    {
        public Texture Texture { get; }

        /// <summary> Size of a single repetition of the texture./// </summary>
        public int PageWidth { get; }

        /// <summary> Size of a single repetition of the texture./// </summary>
        public int PageHeight { get; }

        public bool RepeatLeft, RepeatRight;
        public bool RepeatUp, RepeatDown;
        
        public Backdrop(Texture source)
        {
            Texture = source ?? throw new Exception("Texture cannot be null!");

            PageWidth = source.Width;
            PageHeight = source.Height;

            Scale = ScaleX = ScaleY = 1;
            RepeatLeft = RepeatRight = RepeatUp = RepeatDown = true;
            tileRect = new XNA.Rectangle(0, 0, PageWidth, PageHeight);
        }

        protected override void OnRender(RenderContext ctx)
        {
            //  transform corners to get camera bounds
            var inverse = Matrix.Invert(ctx.RenderTransform);
            float maxWidth = ctx.ViewportWidth, maxHeight = ctx.ViewportHeight;
            Vector2
                tl = Vector2.Transform(new Vector2(0, 0), inverse),
                tr = Vector2.Transform(new Vector2(maxWidth, 0), inverse),
                br = Vector2.Transform(new Vector2(maxWidth, maxHeight), inverse),
                bl = Vector2.Transform(new Vector2(0, maxHeight), inverse);

            //  left, top, right, bottom
            int l = (int) Math.Min(tl.X, Math.Min(tr.X, Math.Min(br.X, bl.X))) / PageWidth - 1,
                t = (int) Math.Min(tl.Y, Math.Min(tr.Y, Math.Min(br.Y, bl.Y))) / PageHeight - 1,
                r = (int) Math.Ceiling(Math.Max(tl.X, Math.Max(tr.X, Math.Max(br.X, bl.X))) / PageWidth),
                b = (int) Math.Ceiling(Math.Max(tl.Y, Math.Max(tr.Y, Math.Max(br.Y, bl.Y))) / PageHeight);

            if (!RepeatLeft)
                l = 0;

            if (!RepeatRight)
                r = 1;

            if (!RepeatDown)
                b = 1;

            if (!RepeatUp)
                t = 0;
            
            for (int tx = l; tx < r; tx++)
            {
                for (int ty = t; ty < b; ty++)
                {
                    var spriteRect = new XNA.Rectangle(
                        tx * PageWidth,
                        ty * PageHeight,
                        PageWidth,
                        PageHeight
                    );

                    ctx.SpriteBatch.Draw(Texture.XnaTexture, spriteRect, tileRect, Color.White);
                }
            }
        }

        private XNA.Rectangle tileRect;
    }
}
