using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo.Engine
{
    public class GameTime
    {
        public float IdealDeltaSeconds { get; }
        public float DeltaSeconds { get; internal set; }
        public float TotalTime { get; internal set; }

        internal GameTime()
        {
            //  TODO: implement custom timestep and make these values dynamic
            IdealDeltaSeconds = 1f / 60f;
            DeltaSeconds = 1f / 60f;
        }
    }
}
