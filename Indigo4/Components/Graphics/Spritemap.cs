using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Indigo.Engine;
using Indigo.Content;
using System.Collections.ObjectModel;
using Indigo.Utility;
using Microsoft.Xna.Framework;
using XNA = Microsoft.Xna.Framework;
using XNAG = Microsoft.Xna.Framework.Graphics;
using Indigo.Engine.Rendering;

namespace Indigo.Components.Graphics
{
    public class Spritemap : Graphic
    {
        public static Frame[] MakeFrames(params int[] indexes)
        {
            return indexes
                .Select(i => new Frame(i))
                .ToArray();
        }

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

        public class Frame
        {
            internal Frame(int index)
            {
                Index = index;
            }

            public int Index { get; }

            public event Action OnBeginFrame;

            internal void CallOnBeginFrame()
            {
                OnBeginFrame?.Invoke();
            }
        }

        public class Animation
        {
            public string Name { get; }

            public bool Loop { get; set; }

            public float FrameRate
            {
                get { return _frameRate; }
                set
                {
                    if (value < 0)
                        throw new ArgumentException("Framerate value cannot be negative!");

                    _frameRate = value;
                }
            }

            public IReadOnlyList<Frame> Frames { get; }

            public Spritemap Spritemap { get; }

            internal Animation(Spritemap spritemap, string name, Frame[] frames)
            {
                Name = name;
                Spritemap = spritemap;
                Frames = new ReadOnlyCollection<Frame>(frames);
            }

            public void Play(bool resetIfPlaying = false, int startOn = 0)
            {
                Spritemap.Play(this, resetIfPlaying, startOn);
            }

            private float _frameRate;
        }


        public bool FlipX
        {
            get => flipX;
            set => Effects = (flipX = value) ? (Effects | XNAG.SpriteEffects.FlipHorizontally) : (Effects & ~XNAG.SpriteEffects.FlipHorizontally);
        }

        public bool FlipY
        {
            get => flipY;
            set => Effects = (flipY = value) ? (Effects | XNAG.SpriteEffects.FlipVertically) : (Effects & ~XNAG.SpriteEffects.FlipVertically);
        }

        public Texture Texture { get; }

        public float Rate { get; set; }

        public int Rows { get; }
        public int Columns { get; }
        public int TotalFrames { get; }

        public int FrameWidth { get; }
        public int FrameHeight { get; }

        public Animation CurrentAnimation { get; private set; }
        public Frame CurrentFrame { get; private set; }

        /// <summary>Whether the current animation has completed. Never set for looping animations.</summary>
        public bool Completed { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="texture">Source texture. Cannot be changed.</param>
        /// <param name="frameWidth">Width of each animation frame.</param>
        /// <param name="frameHeight">Height of each animation frame.</param>
        public Spritemap(Texture texture, int frameWidth, int frameHeight )
        {
            Texture = texture;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;

            rect = new XNA.Rectangle(0, 0, frameWidth, frameHeight);
            
            Columns = (int) Math.Ceiling((float) Texture.Width / frameWidth);
            Rows = (int) Math.Ceiling((float) Texture.Height / frameHeight);
            TotalFrames = Columns * Rows;

            Active = true;
            Rate = 1;
        }
        
        /// <summary>
        /// Adds an animation to the Spritemap.
        /// </summary>
        /// <param name="name">The name of the animation. Must be unique.</param>
        /// <param name="frames">The frames that compose the animation. See static utility methods.</param>
        /// <returns>An Animation instance for controlling additional parameters</returns>
        public Animation AddAnimation(string name, Frame[] frames)
        {
            if (animations.ContainsKey(name))
                throw new Exception($"An animation with the name '{name}' already exists!");

            var result = new Animation(this, name, frames);
            animations[name] = result;
            return result;
        }

        /// <summary>
        /// Adds an animation to the Spritemap.
        /// </summary>
        /// <param name="name">An enum to use as the animation's name. Converted to string internally.</param>
        /// <param name="frames">The frames that compose the animation. See static utility methods.</param>
        /// <returns>An Animation instance for controlling additional parameters</returns>
        public Animation AddAnimation(Enum name, Frame[] frames)
        {
            return AddAnimation(name.ToString(), frames);
        }

        /// <summary>
        /// Play an animation, looking it up by an enum value.
        /// </summary>
        /// <param name="animName">The enum identifying the animation</param>
        /// <param name="resetIfPlaying">Whether the animation should reset if it is already playing</param>
        /// <param name="startOn">The frame to start playing from. Must belong to the named animation.</param>
        public void Play(Enum animName, bool resetIfPlaying, int startOn = 0)
        {
            Play(animName.ToString(), resetIfPlaying, startOn);
        }

        /// <summary>
        /// Play an animation, looking it up by name.
        /// </summary>
        /// <param name="animName">The name of the animation</param>
        /// <param name="resetIfPlaying">Whether the animation should reset if it is already playing</param>
        /// <param name="startOn">The frame to start playing from. Must belong to the named animation.</param>
        public void Play(string animName, bool resetIfPlaying = false, int startOn = 0)
        {
            if (!animations.TryGetValue(animName, out var anim))
                throw new Exception($"No animation with the name {animName} exists!");

            Play(anim, resetIfPlaying, startOn);
        }

        /// <summary>
        /// Play an animation directly.
        /// </summary>
        /// <param name="anim">The animation to play</param>
        /// <param name="resetIfPlaying">Whether the animation should reset if it is already playing</param>
        /// <param name="startOn">The frame to start playing from</param>
        public void Play(Animation anim, bool resetIfPlaying = false, int startOn = 0)
        {
            if (anim == null)
                throw new ArgumentNullException(nameof(anim));

            if (anim.Spritemap != this)
                throw new Exception("Animation does not belong to this spritemap!");

            if (CurrentAnimation == anim && !resetIfPlaying)
                return;
            
            _index = startOn;
            _time = 0;
            CurrentAnimation = anim;
            Completed = false;
            ForceFrame(anim.Frames[0]);
        }

        /// <summary>Stop the currently-playing animation and leave the image in its current state.</summary>
        public void Stop()
        {
            CurrentAnimation = null;
        }

        /// <summary>
        /// Get a frame based on its position in the source texture.
        /// </summary>
        /// <param name="column">X position of frame</param>
        /// <param name="row">Y position of frame</param>
        /// <returns>A new Frame instance corresponding to the given coordinates</returns>
        public Frame GetFrame(int column, int row)
        {
            return new Frame((row % Rows) * Columns + (column % Columns));
        }

        /// <summary>
        /// Stops the currently playing animation and sets the frame based on a position in the source texture.
        /// </summary>
        /// <param name="column">X position of the frame to set</param>
        /// <param name="row">Y position of the frame to set</param>
        public void ForceFrame(int column, int row)
        {
            ForceFrame(GetFrame(column, row));
        }
        
        /// <summary>
        /// Stops the currently playing animation and sets the frame
        /// </summary>
        /// <param name="frame">The frame to force</param>
        public void ForceFrame(Frame frame)
        {
            if (frame == null)
                throw new ArgumentNullException(nameof(frame));

            CurrentAnimation = null;
            SetFrame(frame.Index);
        }

        public override void Update()
        {
            base.Update();

            if (CurrentAnimation == null || Completed)
                return;

            _time += CurrentAnimation.FrameRate * Rate * App.GameTime.DeltaSeconds;

            if (_time >= 1)
            {
                int elapsedFrames = (int)_time;
                _time -= elapsedFrames;
                _index += elapsedFrames;

                var frames = CurrentAnimation.Frames;
                if (_index >= frames.Count)
                {
                    if (CurrentAnimation.Loop) _index %= frames.Count;
                    else
                    {
                        _index = frames.Count - 1;
                        Completed = true;
                    }
                }

                CurrentFrame = frames[_index];
                CurrentFrame.CallOnBeginFrame();
                SetFrame(CurrentFrame.Index);
            }
        }

        protected override void OnRender(RenderContext ctx)
        {
            ctx.SpriteBatch.Draw(Texture.XnaTexture, XNA.Vector2.Zero,rect, Color.White);
        }
        
        private void SetFrame(int index)
        {
            rect.X = FrameWidth * (index % Columns);
            rect.Y = FrameHeight * (index / Columns);
        }

        private float _time;
        private int _index;
        private Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
        private XNA.Rectangle rect;
        private bool flipX, flipY;
        private XNAG.SpriteEffects Effects;

    }
}
