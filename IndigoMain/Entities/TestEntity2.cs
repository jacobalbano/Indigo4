using FNT;
using Indigo;
using Indigo.Components;
using Indigo.Components.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndigoMain.Entities
{
    public class TestEntity2 : Entity
    {
        public TestEntity2()
        {
            var font = App.Library.Load<FontLibrary>("arial.ttf");
            var face = font.CreateFont(32);
            var text = Components.Add(new Text(face) { String = "Tell me why you did it\nEvery dream fallin' apart" });
            text.Y = 100;

            var scaleWave = Components.Add(new Wave(3, 0.75f, 1.25f));
            Components.Add(new Actor(() => text.Scale = scaleWave.Value));
            Components.Add(new Actor(() => text.Angle++));

            X = 100;
        }
    }
}
