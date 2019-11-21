using Alexandria;
using Indigo.Content.Loaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

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
            Renderer.RenderContext.Pixel.SetData(new[] { Color.White });

            App.Library.AddLoader(new TextureLoader(GraphicsDevice));
            App.Library.AddLoader(new FontLoader(GraphicsDevice));
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            Renderer.RenderContext.SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        internal void InitializeSubsystems(SubSystems ss)
        {
            ss.Window = new Window(AppConfig, this);
            ss.Library = new Library();
            ss.Renderer = Renderer;
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            App.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            var target = Renderer.Target;
            var sb = Renderer.RenderContext.SpriteBatch;

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

                sb.Begin(SpriteSortMode.Immediate, null, null, null, null, null, res);
                sb.Draw(target, new Vector2(0, 0), Color.White);
                sb.End();
            }
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
