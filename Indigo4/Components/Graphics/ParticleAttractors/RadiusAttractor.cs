using Indigo.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Indigo.Components.Graphics.ParticleAttractors
{
    /// <summary>Attractor that draws particles towards (or away) from a specified point</summary>
    public class RadiusAttractor : ParticleAttractor
    {
        /// <summary>Attractor position (relative to the Emitter)</summary>
        public float X, Y;

        /// <summary>Threshold within which particles will be affected.</summary>
        public float InfluenceDistance;

        /// <summary>
        /// Degree to which particles will take distance from this attractor into account.
        /// 1 (default) means that the full force will be applied regardless of distance
        /// 0 means that no force will be applied whatsoever
        /// Values between 1 and 0 will result in a blend: 
        /// ((InfluenceDistance - distToParticle) * InfluenceFalloff) / InfluenceDistance
        /// </summary>
        public float InfluenceFalloff = 1;

        public override void GetSteeringSeekPoint(float particleX, float particleY, out float seekX, out float seekY, out float forceRatio)
        {
            forceRatio = ((InfluenceDistance - MathUtility.Distance(X, Y, particleX, particleY)) * InfluenceFalloff) / InfluenceDistance;
            seekX = X;
            seekY = Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool ShouldActOnParticle(float particleX, float particleY)
        {
            //  fast squared distance check
            return MathUtility.DistanceSquared(X, Y, particleX, particleY) < InfluenceDistance * InfluenceDistance;
        }
    }
}
