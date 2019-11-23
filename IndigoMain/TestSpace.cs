using Indigo;
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
            Entities.Add(new TestEntity1());
        }

        public override void Begin()
        {
            base.Begin();
        }
    }
}
