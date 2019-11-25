using Alexandria;
using Glide;
using Indigo.Configuration;
using Indigo.Configuration.Modules.Alexandria;
using Indigo.Configuration.Modules.Default;
using Indigo.Core.Logging;
using Indigo.Engine;
using Indigo.Engine.Implementation;
using Indigo.Inputs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo
{
    public class App
    {
        public static Space NextSpace { get; set; }
        public static Space CurrentSpace { get; private set; }

        public static Logger Log => SS.Logger;
        public static Window Window => SS.Window;
        public static Renderer Renderer => SS.Renderer;
        public static GameTime GameTime => SS.GameTime;

        public static Library Library => SS.Library;

        public static Tweener Tweener => SS.Tweener;

        public static Mouse Mouse => SS.Mouse;

        [Module(typeof(DefaultModule))]
        public class Config : ConfigBase<App>
        {
            public Logger.Config LoggerConfig { get; set; }
            public AlexandriaConfig LibraryConfig { get; set; }
            public Renderer.Config RendererConfig { get; set; }
            public Window.Config WindowConfig { get; set; }

            public Config()
            {
                LoggerConfig = new Logger.Config();
                LibraryConfig = new AlexandriaConfig();
                RendererConfig = new Renderer.Config();
                WindowConfig = new Window.Config();
            }

            public override void Validate()
            {
                if (RendererConfig == null) throw new Exception("RendererConfig must not be null");
                if (WindowConfig == null) throw new Exception("WindowConfig must not be null");

                RendererConfig.Validate();
                WindowConfig.Validate();
            }
        }

        public App(Config config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            config.Validate();
        }

        public virtual void Begin()
        {
        }

        public virtual void Update()
        {
            if (NextSpace != null)
            {
                var next = NextSpace;
                if (CurrentSpace != null)
                    CurrentSpace.End();
                NextSpace = null;

                CurrentSpace = next;
                CurrentSpace.Begin();
            }

            CurrentSpace?.Update();
        }

        public virtual void Render()
        {
            if (CurrentSpace != null)
                Renderer.RenderSpace(CurrentSpace);
        }

        public virtual void End()
        {
        }

        public void Run(Space firstSpace)
        {
            if (SS != null) throw new Exception("Only one App can run at a time");

            using (var game = new XnaGame(this, config))
            using (SS = game.InitializeSubsystems())
            {
                NextSpace = firstSpace;
                game.Run();
            }

            SS = null;
        }

        private readonly Config config;
        private static SubSystems SS;

    }
}
