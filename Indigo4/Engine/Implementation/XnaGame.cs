using Alexandria;
using Indigo.Content.Loaders;
using Microsoft.Xna.Framework;
using XNA = Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Indigo.Engine.Implementation
{
    internal class XnaGame : Game
    {
        public App App { get; }
        public App.Config AppConfig { get; }
        public Renderer Renderer { get; }

        internal GraphicsDeviceManager Graphics { get; private set; }

        public XnaGame(App app, App.Config config)
        {
            App = app;
            AppConfig = config;

            var res = config.RendererConfig.InternalResolution ?? config.WindowConfig.WindowSize;
            Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = (int)res.Width,
                PreferredBackBufferHeight = (int)res.Height,
                SynchronizeWithVerticalRetrace = config.RendererConfig.VSync
            };

            Renderer = new Renderer(config.RendererConfig, this);

            Window.AllowUserResizing = config.WindowConfig.AllowResize;
            IsMouseVisible = config.WindowConfig.ShowCursor;
        }

        #region XNA overrides
        protected override void Initialize()
        {
            base.Initialize();

            var size = AppConfig.RendererConfig.InternalResolution ?? AppConfig.WindowConfig.WindowSize;
            Renderer.Target = new RenderTarget2D(GraphicsDevice, (int) size.Width, (int) size.Height);

            App.Library.AddLoader(new TextureLoader(GraphicsDevice));
            App.Library.AddLoader(new FontLoader(GraphicsDevice));
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        internal void InitializeSubsystems(SubSystems ss)
        {
            ss.Window = new Window(AppConfig, this);
            ss.Library = AppConfig.LibraryConfig.InstantiateObject();
            ss.Renderer = Renderer;
            ss.GameTime = new GameTime();
        }

        protected override void Update(XNA.GameTime gameTime)
        {
            base.Update(gameTime);
            var sw = Stopwatch.StartNew();
            App.Update();
            Console.Write("Update: {0}ms   ", sw.ElapsedMilliseconds);
        }

        protected override void Draw(XNA.GameTime gameTime)
        {
            base.Draw(gameTime);
            var sw = Stopwatch.StartNew();

            App.GameTime.TotalTime = (float) gameTime.TotalGameTime.TotalSeconds;

            var target = Renderer.Target;

            //  Draw entities to the buffer
            {
                GraphicsDevice.SetRenderTarget(target);
                Renderer.Clear();
                App.Render();
            }

            //  Draw buffer to screen, transforming it to fit the window size
            {
                GraphicsDevice.SetRenderTarget(null);

                //  order of operations is important here for some reason;
                //  this has to happen after SetRenderTarget(null)
                var res = GetVirtualRes(
                    target.Width,
                    target.Height,
                    GraphicsDevice.Viewport.Width,
                    GraphicsDevice.Viewport.Height
                );

                spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, res);
                spriteBatch.Draw(target, new Vector2(0, 0), Color.White);
                spriteBatch.End();
            }

            Console.WriteLine("Render: {0}ms", sw.ElapsedMilliseconds);
        }

        protected override void BeginRun()
        {
            App.Begin();
        }

        protected override void EndRun()
        {
            App.End();
        }

        #endregion

        private SpriteBatch spriteBatch;
        private static Matrix GetVirtualRes(float targetWidth, float targetHeight, float currentWidth, float currentHeight)
        {
            float windowRatio = currentWidth / currentHeight;
            float viewRatio = targetWidth / targetHeight;
            float posX = 0, posY = 0;

            if (windowRatio < viewRatio)
            {
                posY = (1 - windowRatio / viewRatio) / 2.0f;
            }
            else
            {
                posX = (1 - viewRatio / windowRatio) / 2.0f;
            }

            var scale = Math.Min(currentWidth / targetWidth, currentHeight / targetHeight);
            return Matrix.CreateScale(scale, scale, 1) * Matrix.CreateTranslation(posX * currentWidth, posY * currentHeight, 0);
        }
    }
}
