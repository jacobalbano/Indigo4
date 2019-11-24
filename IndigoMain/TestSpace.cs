using Indigo;
using Indigo.Utility;
using IndigoMain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndigoMain
{
    public class TestSpace : Space
    {
        public TestSpace()
        {
        }

        public override void Begin()
        {
            base.Begin();

            var rng = new SeededRandom();
            //for (int i = 0; i < 1_000; i++)
            //    Entities.Add(new TestEntity1() { X = rng.Float(App.Window.Size.Width), Y = rng.Float(App.Window.Size.Height) });


            //Entities.Add(new TestEntity2());
            Entities.Add(new TestEntity3());
        }
    }
}
