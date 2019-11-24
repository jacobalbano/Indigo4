
using System;
using System.Collections.Generic;
using Indigo.Content;
using Microsoft.Xna.Framework;
using Indigo.Utility;
using Indigo.Engine;
using System.Linq;
using XNA = Microsoft.Xna.Framework;
using System.Diagnostics;
using Indigo.Engine.Rendering;

namespace Indigo.Components.Graphics
{
    /// <summary>
    /// Particle emitter used for emitting and rendering particle sprites.
    /// Good rendering performance with large amounts of particles.
    /// </summary>
    public partial class Emitter : Graphic
    {
        /// <summary>
        /// Produces a list of frames given a series of sprite indexes.
        /// </summary>
        /// <param name="indexes">Sprite indexes, beginning with 0 for the first sprite in the sheet, and increasing left-right, top-bottom</param>
        public static Frame[] MakeFrames(params int[] indexes)
        {
            return indexes
                .Select(i => new Frame(i))
                .ToArray();
        }

        /// <summary>
        /// Produces a list of Frames given a start and end value. Handles reverse ranges 
        /// </summary>
        /// <param name="start">Range start (can be larger than end parameter)</param>
        /// <param name="end">Range end (can be smaller than start parameter)</param>
        /// <param name="skip">Number of frames to skip between each result</param>
        public static Frame[] MakeFrameRange(int start, int end, int skip = 0)
        {
            return Range()
                .Select(i => new Frame(i))
                .ToArray();

            IEnumerable<int> Range()
            {
                int count = Math.Abs(end - start);
                int sign = Math.Sign(end - start);

                for (int i = 0; i < count + 1; i += skip + 1)
                    yield return start + (sign * i);
            }
        }

        /// <summary>Class </summary>
        public class Frame
        {
            internal Frame(int index)
            {
                Index = index;
            }

            public int Index { get; }
        }

        /// <summary>Amount of currently active particles.</summary>
        public int ParticleCount { get; private set; }

        /// <summary>Random number generator for this instance of the emitter. Defaults to a shared static instance.</summary>
        public SeededRandom Random = SeededRandom.Default;

        public IEnumerable<ParticleDefinition> Definitions => definitions.Values;

        /// <summary>Texture assigned to this emitter. Cannot be changed.</summary>
        public Texture Texture { get; }

        public Emitter(int particleWidth, int particleHeight)
        {
            Active = true;
            definitions = new Dictionary<string, ParticleDefinition>();
        }

        /// <summary>Constructor. Will produce an Emitter with no texture (particles will be blank rectangles)</summary>
        public Emitter()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">Texture to assign to the emitter.</param>
        public Emitter(Texture source)
        {
            Texture = source;
        }

        /// <summary>
        /// Define a particle type
        /// </summary>
        /// <param name="width">Frame width</param>
        /// <param name="height">Frame height</param>
        /// <returns>The newly defined particle type</returns>
        public ParticleDefinition Define(int width, int height)
        {
            return Define(Guid.NewGuid().ToString("N"), width, height);
        }

        /// <summary>
        /// Define a particle type
        /// </summary>
        /// <param name="name">Type name</param>
        /// <param name="width">Frame width</param>
        /// <param name="height">Frame height</param>
        /// <returns>The newly defined particle type</returns>
        public ParticleDefinition Define(string name, int width, int height)
        {
            return Define(name, width, height, noFrames);
        }

        /// <summary>
        /// Define a particle type that animates using the Emitter's texture and a supplied list of frames
        /// </summary>
        /// <param name="name">Type name</param>
        /// <param name="width">Frame width</param>
        /// <param name="height">Frame height</param>
        /// <param name="frames">Animation frames</param>
        /// <returns>The newly defined particle type</returns>
        public ParticleDefinition Define(string name, int width, int height, Frame[] frames)
        {
            if (definitions.TryGetValue(name, out _))
                throw new Exception($"A particle definition named '{name}' already exists!");

            int cols = Texture?.Width / width ?? 1;
            var def = new ParticleDefinition(this, name, frames) { frameWidth = width, frameHeight = height, frameColumns = cols};
            definitions[name] = def;
            return def;
        }
        
        /// <summary>
        /// Update the graphic.
        /// </summary>
        public override void Update()
        {
            base.Update();

            // quit if there are no particles
            if (FirstActive == null)
                return;

            Simulate(App.GameTime.DeltaSeconds);
        }

        protected override void OnRender(RenderContext ctx)
        {
            // particle info
            var point = new Vector2();
            var origin = new Vector2();
            var color = new Color();

            // loop through the particles
            for (var p = FirstActive; p != null; p = p.next)
            {
                // get time scale
                if (p.duration == 0) continue;

                float td = 0;
                float t = p.time / p.duration;
                float scale, angle, alpha;

                // get particle type
                var def = p.Definition;
                int frameWidth = def.frameWidth, frameHeight = def.frameHeight;

                // get position
                td = def.Motion.Ease?.Invoke(t) ?? t;
                point.X = RelativeX + p.x;
                point.Y = RelativeY + p.y;

                int frameX = 0, frameY = 0;

                if (Texture == null)
                {
                    origin.X = 1f / def.OriginX;
                    origin.Y = 1f / def.OriginY;
                }
                else
                {
                    // get frame
                    int frameIndex = (int)(t * def.frames.Length);
                    int spriteIndex = def.frames[frameIndex].Index;
                    int width = Texture.Width;
                    frameX = frameWidth * spriteIndex;
                    frameY = (frameX / width) * frameWidth;
                    frameX %= width;

                    origin.X = def.OriginX;
                    origin.Y = def.OriginY;
                }

                // get scale
                var s = def.Scale;
                td = s.Ease?.Invoke(t) ?? t;
                scale = p.scaleRand + s.From + (s.To - s.From) * td;

                var rot = def.Angle;
                td = rot.Ease?.Invoke(t) ?? t;
                angle = p.angleRand + rot.From + (rot.To - rot.From) * td;
                if (angle < 0) angle += 360;

                // get alpha
                var a = def.Alpha;
                td = a.Ease?.Invoke(t) ?? t;
                alpha = p.alphaRand + a.From + (a.To - a.From) * td;

                // get color
                td = MathHelper.Clamp(def.Color.Ease?.Invoke(t) ?? t, 0, 1);

                var from = def.Color.From.R << 16 | def.Color.From.G << 8 | def.Color.From.B;
                var to = def.Color.To.R << 16 | def.Color.To.G << 8 | def.Color.To.B;
                var r = from >> 16 & 0xFF;
                var g = from >> 8 & 0xFF;
                var b = from & 0xFF;
                var startR = r / 255f;
                var startG = g / 255f;
                var startB = b / 255f;
                var rangeR = ((to >> 16 & 0xFF) / 255f) - startR;
                var rangeG = ((to >> 8 & 0xFF) / 255f) - startG;
                var rangeB = ((to & 0xFF) / 255f) - startB;

                color.R = (byte)((startR + rangeR * td) * 255);
                color.G = (byte)((startG + rangeG * td) * 255);
                color.B = (byte)((startB + rangeB * td) * 255);
                color.A = (byte)(255 * MathHelper.Clamp(alpha, 0, 1));
                    
                var rect = new XNA.Rectangle(frameX, frameY, frameWidth, frameHeight);
                ctx.SpriteBatch.Draw(Texture?.XnaTexture ?? ctx.Pixel, point, rect, color, MathUtility.RAD * angle, origin, scale, XNA.Graphics.SpriteEffects.None, 0);
            }
        }

        /// <summary>Precache a given number of particles into the Emitter to save on constructing later.</summary>
        public void Precache(int count)
        {
            if (FirstCache == null)
            {
                FirstCache = new Particle();
                count--;
            }

            //  holy crap look at this baby
            for (int i = 0; i < count; i++)
                FirstCache = new Particle { next = FirstCache };
        }

        /// <summary>
        /// Emits a particle.
        /// </summary>
        /// <param name="type">The particle type to emit</param>
        /// <param name="position">Point to emit from.</param>
        public void Emit(ParticleDefinition type, Point position)
        {
            Emit(type, position.X, position.Y);
        }

        /// <summary>
        /// Emits a particle.
        /// </summary>
        /// <param name="type">The particle type to emit</param>
        /// <param name="x">X point to emit from.</param>
        /// <param name="y">Y point to emit from.</param>
        public void Emit(ParticleDefinition type, float x, float y)
        {
            if (type.Owner != this)
                throw new Exception("Particle type does not belong to this emitter!");

            var p = CreateParticle();

            p.next = FirstActive;
            p.prev = null;
            if (p.next != null) p.next.prev = p;

            p.Definition = type;
            p.time = 0;

            p.duration = type.Lifetime.Duration;
            p.duration += type.Lifetime.Variance.Add * Random.Float();
            p.duration -= type.Lifetime.Variance.Subtract * Random.Float();
            if (p.duration == 0)
                p.duration = DT;

            float angle = type.Angle.From;
            angle += type.Angle.Variance.Add * Random.Float();
            angle -= type.Angle.Variance.Subtract * Random.Float();

            var moveAngle = type.Motion.Angle;
            moveAngle += type.Motion.AngleVariance.Add * Random.Float();
            moveAngle -= type.Motion.AngleVariance.Subtract * Random.Float();

            var distance = type.Motion.Distance;
            distance += type.Motion.DistanceVariance.Add * Random.Float();
            distance -= type.Motion.DistanceVariance.Subtract * Random.Float();
            
            p.initialVelocity = (distance * DT) / p.duration; // assuming XNA fixed is 60fps

            p.x = x;
            p.y = y;
            MathUtility.AngleXY(ref p.velX, ref p.velY, moveAngle, p.initialVelocity);

            p.alphaRand += type.Alpha.Variance.Add * Random.Float();
            p.alphaRand -= type.Alpha.Variance.Subtract * Random.Float();

            p.scaleRand += type.Scale.Variance.Add * Random.Float();
            p.scaleRand -= type.Scale.Variance.Subtract * Random.Float();

            p.angleRand += type.Angle.Variance.Add * Random.Float();
            p.angleRand -= type.Angle.Variance.Subtract * Random.Float();

            p.accelX = type.Motion.Acceleration.X * DT;
            p.accelY = type.Motion.Acceleration.Y * DT;

            ParticleCount++;
            FirstActive = p;
        }

        /// <summary>
        /// Emits a particle.
        /// </summary>
        /// <param name="particleTypeName">The name of the particle type to emit</param>
        /// <param name="x">X point to emit from.</param>
        /// <param name="y">Y point to emit from.</param>
        public void Emit(string particleTypeName, float x, float y)
        {
            if (!definitions.TryGetValue(particleTypeName, out var def))
                throw new Exception($"No particle definition with the name '{particleTypeName}' exists!");

            Emit(def, x, y);
        }

        /// <summary>Stops all particles and removes them from the emitter (objects are still cached)</summary>
        public void Clear()
        {
            Particle p = FirstActive, n = null;

            // for (var p = FirstActive; p != null; p = p.next)
            while (p != null)
            {
                if (p.next != null) p.next.prev = p.prev;
                if (p.prev != null) p.prev.next = p.next;
                else FirstActive = p.next;

                n = p.next;
                p.next = FirstCache;
                p.prev = null;
                FirstCache = p;
                p = n;
                if (p == null) return;

                ParticleCount--;
                p = p.next;
            }
        }

        /// <summary>
        /// <para>Simulate the emitter running for a given amount of time.</para>
        /// </summary>
        /// <param name="deltaSeconds">The time to run for.</param>
        public void Simulate(float deltaSeconds)
        {
            var p = FirstActive;

            // loop through the particles
            while (p != null)
            {
                var type = p.Definition;

                // update time scale
                p.time += deltaSeconds;
                float t = p.time / p.duration;
                float td = type.Motion.Ease?.Invoke(t) ?? t;
                p.x += p.velX + p.accelX * td * td;
                p.y += p.velY + p.accelY * td * td;

                var position = new Vector2(p.x, p.y);
                float maxVelocity = p.initialVelocity;

                for (int i = 0; i < type.Attractors.Count; i++)
                {
                    var a = type.Attractors[i];
                    if (!a.ShouldActOnParticle(position.X, position.X))
                        continue;
                    
                    var target = new Vector2();
                    a.GetSteeringSeekPoint(p.x, p.y, out target.X, out target.Y, out float forceRatio);
                    
                    float maxForce = p.initialVelocity * (a.SteeringForce * forceRatio);
                    maxVelocity = Math.Max(maxVelocity, p.initialVelocity * a.Acceleration);
                    
                    var velocity = new Vector2(p.velX, p.velY);
                    var desiredVel = Vector2.Normalize(target - position) * maxVelocity;

                    var steering = Truncate(desiredVel - velocity, maxForce);
                    velocity = Truncate(velocity + steering, maxVelocity);

                    p.velX = velocity.X;
                    p.velY = velocity.Y;
                }
                
                // remove on time-out
                if (p.duration == 0 || p.time >= p.duration)
                {
                    p.Definition._onComplete(p.x, p.y);
                    if (p.next != null) p.next.prev = p.prev;
                    if (p.prev != null) p.prev.next = p.next;
                    else FirstActive = p.next;

                    var n = p.next;
                    p.next = FirstCache;
                    p.prev = null;
                    FirstCache = p;
                    p = n;
                    ParticleCount--;
                    continue;
                }

                // get next particle
                p = p.next;
            }

            Vector2 Truncate(Vector2 v, float max)
            {
                if (v.LengthSquared() > max * max)
                    v = Vector2.Normalize(v) * max;

                return v;
            }
        }

        private Particle CreateParticle()
        {
            Particle p = null;

            if (FirstCache != null)
            {
                p = FirstCache;
                FirstCache = p.next;

                p.x = 0;
                p.y = 0;
                p.accelX = 0;
                p.accelY = 0;
                p.velX = 0;
                p.velY = 0;
                p.initialVelocity = 0;
                p.time = p.duration = 0;
                p.scaleRand = p.alphaRand = p.angleRand = 0;
            }

            return p ?? new Particle();
        }

        private ParticleDefinition Add(ParticleDefinition def)
        {
            if (definitions.ContainsKey(def.Name))
                throw new Exception(string.Format("A ParticleDefinition named '{0}' has already been created.", def.Name));

            definitions[def.Name] = def;
            return def;
        }

        // Particle information.
        private Particle FirstActive;
        private Particle FirstCache;

        private Dictionary<string, ParticleDefinition> definitions = new Dictionary<string, ParticleDefinition>();
        private readonly Frame[] noFrames = MakeFrames(0);

        private const float DT = 1 / 60f;

        private class Particle
        {
            public ParticleDefinition Definition;
            public float time, duration;
            public float x, y, velX, velY, accelX, accelY;
            public float alphaRand, scaleRand, angleRand;
            public Particle prev, next;
            public float initialVelocity;
        }
    }
}
