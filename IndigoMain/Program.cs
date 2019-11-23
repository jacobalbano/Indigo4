using Indigo;
using Indigo.Core;
using Indigo.Core.Collections;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndigoMain
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new App.Config
            {
                RendererConfig = { ClearColor = Color.Indigo },
                WindowConfig =
                {
                    WindowSize = new Size { Width = 640, Height = 480 }
                }
            };

            var app = new App(config);
            App.NextSpace = new TestSpace();
            app.Run();
        }
    }
}
