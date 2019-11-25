using Indigo;
using Indigo.Components;
using Indigo.Components.Graphics;
using Indigo.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNA = Microsoft.Xna.Framework;

namespace IndigoMain.Entities
{
    public class TestEntity3 : Entity
    {
        public TestEntity3()
        {
            X = Y = 200;

            var parent = new Rectangle { Width = 100, Height = 100, Color = XNA.Color.Blue };
            var child = new Rectangle { Width = 100, Height = 100, Color = XNA.Color.Red };

            child.X = 100;
            child.Y = 100;

            Components.Add(parent);
            Components.Add(child);

            Layer = new EntityLayer("Entity3") { Depth = -10 };

            var wave = Components.Add(new Wave(3, 0f, 1f));
            Components.Add(new Actor(() => parent.Y = 100 * wave.Value));
            //Components.Add(new Actor(() => parent.Angle++));
            //Components.Add(new Actor(() => child.Angle++));
        }

        public override void Update()
        {
            base.Update();

            var buttons = new[]
            {
                new { Name = "Left", Button = App.Mouse.Left },
                new { Name = "Right", Button = App.Mouse.Right },
                new { Name = "Any", Button = App.Mouse.Any },
            };

            foreach (var pair in buttons)
            {
                if (pair.Button.Pressed) App.Log.WriteLine($"{pair.Name} pressed");
                if (pair.Button.Released) App.Log.WriteLine($"{pair.Name} released");
            }
        }
    }
}
