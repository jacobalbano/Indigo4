using System;
using Indigo;
using Indigo.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace IndigoTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
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
            app.Run();
        }
    }
}
