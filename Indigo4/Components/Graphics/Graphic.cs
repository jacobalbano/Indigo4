using System;
using Indigo.Core;
using Indigo.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Indigo.Utility;
using Indigo.Engine.Rendering;

namespace Indigo.Components.Graphics
{
    public abstract class Graphic : Component
    {
        protected struct XnaValues
        {
            public Vector2 Position;
            public float Rotation;
            public Vector2 Origin;
            public Vector2 Scale;
        }

        public bool RelativeToEntity = true;

        public Transform Transform
        {
            get => transform;
            set => transform = value ?? throw new ArgumentNullException("Transform");
        }

        public float X { get => Transform.X; set => Transform.X = value; }
        public float Y { get => Transform.Y; set => Transform.Y = value; }
        public float Angle { get => Transform.Angle; set => Transform.Angle = value; }
        public float Scale { get => Transform.Scale; set => Transform.Scale = value; }
        public float ScaleX { get => Transform.ScaleX; set => Transform.ScaleX = value; }
        public float ScaleY { get => Transform.ScaleY; set => Transform.ScaleY = value; }
        public float OriginX { get => Transform.OriginX; set => Transform.OriginX = value; }
        public float OriginY { get => Transform.OriginY; set => Transform.OriginY = value; }

        public float RelativeX => RelativeToEntity ? X + (Entity?.X ?? 0) : X;
        public float RelativeY => RelativeToEntity ? Y + (Entity?.Y ?? 0) : Y;

        public Effect Effect { get; set; }

        private Transform transform = new Transform();

        public sealed override void Render(RenderContext ctx)
        {
            ctx.PrepareForGraphic(this);
            OnRender(ctx);
            ctx.End();
        }

        protected abstract void OnRender(RenderContext ctx);
    }
}
