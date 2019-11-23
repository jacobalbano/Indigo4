using Indigo.Core;
using System;
using Microsoft.Xna.Framework;

namespace Indigo.Components
{
    public class Wave : Component
    {
        public float To, From, LoopDuration, OffsetPercent;
        public float Value
        {
            get
            {
                float ms = LoopDuration / 1000f, tms = TotalTime / 1000f;
                return From + Range + (float)Math.Sin(((tms + ms * OffsetPercent) / ms) * (Math.PI * 2)) * Range;
            }
        }

        private float Range, TotalTime;

        public Wave(float loopDuration, float from, float to, float offsetPercent = 0)
        {
            LoopDuration = loopDuration;
            From = from;
            To = to;
            OffsetPercent = offsetPercent;
            Range = (to - from) / 2;
        }

        public override void Update()
        {
            base.Update();
            TotalTime += App.GameTime.DeltaSeconds;
        }
    }
}
