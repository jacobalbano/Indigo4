using Indigo.Configuration;
using Indigo.Core;
using Indigo.Engine.Implementation;
using Indigo.Engine.Rendering;
using Indigo.Modules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo.Engine
{
    public class Renderer : IConfigurableObject
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

        internal RenderContext RenderContext { get; }
        internal RenderTarget2D Target { get; set; }

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

            RenderContext = new RenderContext()
            {
                Pixel = new Texture2D(gameImpl.GraphicsDevice, 1, 1)
            };
        }

        public void Clear()
        {
            graphicsDevice.Clear(ClearColor);
        }

        private GraphicsDevice graphicsDevice;
        private GraphicsDeviceManager graphicsDeviceManager;
        private Size size;
        private bool vsync;
    }
}
