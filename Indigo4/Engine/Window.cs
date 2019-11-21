using Indigo.Configuration;
using Indigo.Core;
using Indigo.Engine.Implementation;
using Indigo.Engine.Rendering;
using Indigo.Modules;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo.Engine
{
    public class Window : IConfigurableObject
    {
        public Size Size
        {
            get { return size; }
            set
            {
                if (value != size)
                {
                    size = value;
                    UpdateXnaSize();
                }
            }
        }

        public bool AllowResize
        {
            get { return XnaGame.Window.AllowUserResizing; }
            set
            {
                if (value != XnaGame.Window.AllowUserResizing)
                    XnaGame.Window.AllowUserResizing = value;
            }
        }

        public bool ShowCursor
        {
            get { return XnaGame.IsMouseVisible; }
            set
            {
                if (value != XnaGame.IsMouseVisible)
                    XnaGame.IsMouseVisible = value;
            }
        }

        [Module(typeof(DefaultModule))]
        public class Config : ConfigBase<Window>
        {
            public bool ShowCursor { get; set; } = true;
            public bool AllowResize { get; set; } = true;

            public Size WindowSize { get; set; }

            public override void Validate()
            {
                base.Validate();

                if (WindowSize == Size.Zero)
                    throw new Exception("Window size cannot be {0, 0}");
            }
        }

        internal Window(App.Config appConfig, XnaGame xnaGame)
        {
            AllowResize = appConfig.WindowConfig.AllowResize;
            ShowCursor = appConfig.WindowConfig.ShowCursor;
            xnaGame.Window.ClientSizeChanged += Window_ClientSizeChanged;
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            var gameWindow = sender as GameWindow;
            var bounds = gameWindow.ClientBounds;
            size = new Size { Width = bounds.Width, Height = bounds.Height };
        }

        private void UpdateXnaSize()
        {
            XnaGame.Graphics.PreferredBackBufferWidth = (int) size.Width;
            XnaGame.Graphics.PreferredBackBufferHeight = (int) size.Height;
            XnaGame.Graphics.ApplyChanges();
        }

        private Size size;
        private readonly XnaGame XnaGame;

    }
}
