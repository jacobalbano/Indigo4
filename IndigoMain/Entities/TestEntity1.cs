using Indigo;
using Indigo.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndigoMain.Entities
{
    public class TestEntity1 : Entity
    {
        private int counter = 10;
        private Actor actor;

        public TestEntity1()
        {
            actor = Components.Add(new Actor(() => Console.WriteLine("Actor updating!")));
        }
        
        public override void Update()
        {
            base.Update();
            Console.WriteLine("TestEntity1 updating!");
            
            if (--counter < 0)
            {
                if (actor.Entity != null)
                {
                    Components.Remove(actor);
                    counter = 10;
                }
                else
                {
                    Space.Entities.Remove(this);
                }
            }
        }
    }
}
