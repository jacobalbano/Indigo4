using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo.Core
{
    public class Transform
    {
        //public Transform Parent { get; set; }

        public float X { get => x; set => Set(ref x, value); }
        public float Y { get => y; set => Set(ref y, value); }
        public float Angle { get => angle; set => Set(ref angle, value); }
        public float Scale { get => scale; set => Set(ref scale, value); }
        public float ScaleX { get => scaleX; set => Set(ref scaleX, value); }
        public float ScaleY { get => scaleY; set => Set(ref scaleY, value); }
        public float OriginX { get => originX; set => Set(ref originX, value); }
        public float OriginY { get => originY; set => Set(ref originY, value); }

        public Transform()
        {
            X = Y = Angle = 0;
            Scale = ScaleX = ScaleY = 1;
            OriginX = OriginY = 0;
        }

        private void Set(ref float storage, float value)
        {
            if (value != storage) dirty = true;
            storage = value;
        }

        public Matrix GetXnaTransform()
        {
            if (dirty)
            {
                dirty = false;
                globalTransform = localTransform = Matrix.CreateTranslation(-originX, -originY, 0)
                    * Matrix.CreateScale(scale * scaleX, scale * scaleY, 1)
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(angle));
            }

            //if (Parent != null)
            //    globalTransform = localTransform * Parent.GetXnaTransform();

            return globalTransform;
        }

        private float x, y, angle, scale, scaleX, scaleY, originX, originY;
        private Matrix globalTransform = Matrix.Identity;
        private Matrix localTransform;
        private bool dirty;
    }
}
