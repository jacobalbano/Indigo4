using Alexandria;
using Glide;
using Indigo.Configuration;
using Indigo.Configuration.Modules;
using Indigo.Engine;
using Indigo.Engine.Implementation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo
{
    public class App : IConfigurableObject
    {
        public static Space NextSpace { get; set; }
        public static Space CurrentSpace { get; private set; }

        public static Window Window => SS.Window;
        public static Renderer Renderer => SS.Renderer;

        public static Library Library => SS.Library;

        public static Tweener Tweener => SS.Tweener;

        [Module(typeof(DefaultModule))]
        public class Config : ConfigBase<App>
        {
            public Renderer.Config RendererConfig { get; set; }

            public Window.Config WindowConfig { get; set; }

            public Config()
            {
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
            //App.Mixer.Update(gameTime);
            //App.Keyboard.Update(IsActive);
            //App.Mouse.Update(IsActive);

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

        }

        public virtual void End()
        {
        }

        public void Run()
        {
            if (SS != null) throw new Exception("Only one App can run at a time");

            using (SS = new SubSystems())
            using (var game = new XnaGame(this, config))
            {
                game.InitializeSubsystems(SS);
                game.Run();
            }

            SS = null;
        }

        private readonly Config config;
        private static SubSystems SS;

    }
}
