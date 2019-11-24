using System;
using Microsoft.Xna.Framework;
using XNA = Microsoft.Xna.Framework;
using System.Collections.Generic;
using Indigo.Components.Graphics.ParticleAttractors;

namespace Indigo.Components.Graphics
{
    public class ParticleDefinition
    {
        public Emitter Owner { get; }

        public float OriginX, OriginY;
        public string Name { get; }

        internal ParticleDefinition(Emitter owner, string name, Emitter.Frame[] frames)
        {
            Owner = owner;

            Name = name;
            this.frames = frames;
            Alpha = new Behavior<float>() { From = 1, To = 1 };
            Color = new Behavior<Color>() { From = XNA.Color.White, To = XNA.Color.White };
            Angle = new Behavior<float>() { From = 0, To = 0 };
            Scale = new Behavior<float>() { From = 1, To = 1 };

            Lifetime = new LifetimeInfo();
            Motion = new MotionInfo();
            OnComplete = delegate { };
            Attractors = new List<ParticleAttractor>();
        }
        
        public delegate void CompleteEvent(float x, float y);
        public event CompleteEvent OnComplete;
        
        public Behavior<float> Alpha { get; private set; }
        public Behavior<Color> Color { get; private set; }
        public Behavior<float> Angle { get; private set; }
        public Behavior<float> Scale { get; private set; }

        public LifetimeInfo Lifetime { get; private set; }
        public MotionInfo Motion { get; private set; }
        public List<ParticleAttractor> Attractors { get; }

        internal Emitter.Frame[] frames;
        internal int frameWidth, frameHeight, frameColumns;

        public class Behavior<T> where T : struct
        {
            public T From, To;
            public Func<float, float> Ease;
            public Variance<T> Variance { get; private set; }

            internal Behavior()
            {
                Variance = new Variance<T>();
            }
        }

        public class LifetimeInfo
        {
            public float Duration;
            public Variance<float> Variance { get; private set; }

            internal LifetimeInfo()
            {
                Variance = new Variance<float>();
            }
        }

        public class MotionInfo
        {
            public float Angle, Distance;
            public Vector2 Acceleration;
            public Variance<float> AngleVariance { get; private set; }
            public Variance<float> DistanceVariance { get; private set; }
            public Func<float, float> Ease;

            internal MotionInfo()
            {
                AngleVariance = new Variance<float>();
                DistanceVariance = new Variance<float>();
            }
        }

        public class Variance<T> where T : struct
        {
            public T Both { set { Add = Subtract = value; } }
            public T Add, Subtract;
            internal Variance() { }
        }

        internal void _onComplete(float x, float y)
        {
            OnComplete.Invoke(x, y);
        }

        public void CenterOrigin()
        {
            OriginX = frameWidth / 2f;
            OriginY = frameHeight / 2f;
        }
    }
}
