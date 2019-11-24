using Indigo.Configuration;
using Indigo.Configuration.Modules.Default;
using Indigo.Core;
using Indigo.Engine.Implementation;
using Indigo.Engine.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo.Engine
{
    public class Renderer
    {
        public Size InternalResolution
        {
            get { return size; }
            set
            {
                if (value != size)
                {
                    size = value;
                    Target.Dispose();
                    Target = new RenderTarget2D(graphicsDevice, (int)size.Width, (int)size.Height);
                }
            }
        }

        public bool VSync
        {
            get { return vsync; }
            set
            {
                if (value != vsync)
                {
                    vsync = value;
                    graphicsDeviceManager.SynchronizeWithVerticalRetrace = value;
                }
            }
        }

        public Color ClearColor { get; set; }

        [Module(typeof(DefaultModule))]
        public class Config : ConfigBase<Renderer>
        {
            public Size? InternalResolution { get; set; }

            public Color ClearColor { get; set; }
            public bool VSync { get; set; }

            public Config()
            {
                ClearColor = Color.Black;
            }
        }

        internal Renderer(Config config, XnaGame gameImpl)
        {
            graphicsDevice = gameImpl.GraphicsDevice;
            graphicsDeviceManager = gameImpl.Graphics;

            //  initial values for vsync and resolution are set in XnaGame
            ClearColor = config.ClearColor;

            spriteBatch = new SpriteBatch(graphicsDevice);
            pixel = new Texture2D(gameImpl.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            renderContext = new RenderContext() { Pixel = pixel };
        }

        internal RenderContext PrepareContext(Camera camera, EntityLayer layer)
        {
            return PrepareContext(camera, layer.ScrollX, layer.ScrollY);
        }

        internal RenderContext PrepareContext(Camera camera)
        {
            return PrepareContext(camera, 1, 1);
        }

        private RenderContext PrepareContext(Camera camera, float scrollX, float scrollY)
        {
            renderContext.SpriteBatch = spriteBatch;
            renderContext.ViewportWidth = InternalResolution.Width;
            renderContext.ViewportHeight = InternalResolution.Height;

            renderContext.CameraTransform = camera.GetTransform(scrollX, scrollY);
            return renderContext;
        }

        internal void RenderSpace(Space space)
        {
            space.Render(this);
        }

        public void Clear()
        {
            graphicsDevice.Clear(ClearColor);
        }

        private GraphicsDevice graphicsDevice;
        private GraphicsDeviceManager graphicsDeviceManager;
        internal RenderTarget2D Target { get; set; }
        private Size size;
        private bool vsync;

        private readonly Texture2D pixel;
        private readonly RenderContext renderContext;
        private readonly SpriteBatch spriteBatch;
    }
}
