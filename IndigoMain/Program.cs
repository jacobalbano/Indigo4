using Indigo;
using Indigo.Configuration.Modules.Alexandria;
using Indigo.Core;
using Indigo.Core.Logging.Endpoints;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace IndigoMain
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new App.Config
            {
                LibraryConfig = { FileStores = { new RootDirectoryFileStoreConfig { RootDirectoryPath = "../../Assets" } } },
                RendererConfig = { ClearColor = Color.Indigo },

                LoggerConfig = {
                    IncludeCallingMethod = true,
                    Endpoints = { new ConsoleEndpoint.Config() }
                },

                WindowConfig = {
                    WindowSize = new Size { Width = 640, Height = 480 }
                }
            };

            var app = new App(config);
            app.Run(firstSpace: new TestSpace());
        }
    }
}
