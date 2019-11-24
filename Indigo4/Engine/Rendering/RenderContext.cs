using System;
using System.Collections.Generic;
using System.Text;
using Indigo.Components.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Indigo.Engine.Rendering
{
    public class RenderContext
    {
        public SpriteBatch SpriteBatch { get; internal set; }
        public Texture2D CurrentTexture { get; internal set; }
        public Texture2D Pixel { get; internal set; }

        public Matrix GraphicTransform { get; internal set; } = Matrix.Identity;
        public Matrix CameraTransform { get; internal set; } = Matrix.Identity;
        public Matrix RenderTransform { get; internal set; } = Matrix.Identity;

        public float ViewportWidth { get; internal set; }
        public float ViewportHeight { get; internal set; }

        public void PrepareForGraphic(Graphic graphic)
        {
            if (isPrepared)
                throw new Exception("RenderContext is already prepared");

            GraphicTransform = graphic.Transform.GetXnaTransform()
                * Matrix.CreateTranslation(graphic.RelativeX, graphic.RelativeY, 0);

            RenderTransform = GraphicTransform * CameraTransform;
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, graphic.Effect, RenderTransform);

            isPrepared = true;
        }

        public void End()
        {
            if (!isPrepared)
                throw new Exception("RenderContext was not prepared");

            SpriteBatch.End();
            isPrepared = false;
        }

        private bool isPrepared = false;
    }
}
