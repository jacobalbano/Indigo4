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
        private class A : Entity
        {
            public override void Update()
            {
                Console.WriteLine("A");
            }
        }

        private class B : Entity
        {
            public override void AddedToSpace()
            {
                Console.WriteLine("B");
                Entities.Add(new C());
            }

            public override void RemovedFromSpace()
            {
                Console.WriteLine("B removed");
            }

            public override void Update()
            {
                Entities.Remove(this);
            }
        }

        private class C : Entity
        {
            public override void AddedToSpace()
            {
                Console.WriteLine("C added");
            }

            public override void Update()
            {
                Console.WriteLine("C");
            }
        }

        static BufferedCollection2<Entity> Entities = new BufferedCollection2<Entity>();

        static void Main(string[] args)
        {
            Entities.Add(new A());
            Console.WriteLine("Updating with A");
            Update();

            Console.WriteLine("Updating with A, B, B");
            Entities.Add(new B());
            Entities.Add(new B());
            Update();

            Console.WriteLine("Updating with A, C");
            Update();

            Console.ReadKey();
            return;
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

        private static void Update()
        {
            foreach (var delta in Entities.SyncYieldingDeltas())
            {
                switch (delta.Operation)
                {
                    case BufferedCollection2<Entity>.DeltaOperation.Add:
                        delta.Item.AddedToSpace();
                        break;
                    case BufferedCollection2<Entity>.DeltaOperation.Remove:
                        delta.Item.RemovedFromSpace();
                        break;
                }
            }

            foreach (var e in Entities)
                e.Update();

            Console.WriteLine("---");
        }
    }
}
