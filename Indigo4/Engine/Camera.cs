using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Indigo.Engine
{
    public class Camera
    {
        public float X { get => x; set => Set(ref x, value); }
        public float Y { get => y; set => Set(ref y, value); }
        public float OriginX { get => originX; set => Set(ref originX, value); }
        public float OriginY { get => originY; set => Set(ref originY, value); }
        public float Zoom { get => zoom; set => Set(ref zoom, value); }
        public float Angle { get => angle; set => Set(ref angle, value); }

        internal Camera()
        {
            Zoom = 1;
        }

        public Matrix GetTransform(float scrollX = 1, float scrollY = 1)
        {
            if (dirty)
            {
                dirty = false;
                matrix = Matrix.CreateTranslation(-OriginX, -OriginY, 0)
                    * Matrix.CreateScale(Zoom, Zoom, 1)
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(Angle))
                    * Matrix.CreateTranslation(new Vector3(OriginX, OriginY, 0));
            }

            return Matrix.CreateTranslation(X * scrollX, Y * scrollY, 0) * matrix;
        }

        private void Set(ref float storage, float value)
        {
            if (value != storage) dirty = true;
            storage = value;
        }

        private float x, y, originX, originY, zoom, angle;
        private bool dirty = true;
        private Matrix matrix;
    }
}