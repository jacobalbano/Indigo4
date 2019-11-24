using Indigo;
using Indigo.Components;
using Indigo.Components.Graphics;
using Indigo.Content;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndigoMain.Entities
{
    public class TestEntity1 : Entity
    {
        public TestEntity1()
        {
            var img = Components.Add(new Image(App.Library.Load<Texture>(@"PQ2_Chie_Satonaka.png")));
            img.CenterOrigin();

            var scaleWave = Components.Add(new Wave(3, 0.75f, 1.25f));
            Components.Add(new Actor(() => img.Scale = scaleWave.Value));
            Components.Add(new Actor(() => img.Angle++));

            var positionWave = Components.Add(new Wave(4, 0.1f, 0.5f));
            Components.Add(new Actor(() =>
            {
                img.X = (float) App.Window.Size.Width * positionWave.Value;
                img.Y = (float) App.Window.Size.Height * positionWave.Value;
            }));
        }
    }
}
