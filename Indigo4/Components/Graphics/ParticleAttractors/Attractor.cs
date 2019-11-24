namespace Indigo.Components.Graphics.ParticleAttractors
{
    public abstract class ParticleAttractor
    {
        /// <summary>
        /// Amount by which this attractor is able to influence the speed of particles. 
        /// Default = 1, which means no change. Set higher to allow particles to be sped up.
        /// </summary>
        public float Acceleration = 1;
        public float SteeringForce = 1;

        public abstract bool ShouldActOnParticle(float particleX, float particleY);
        public abstract void GetSteeringSeekPoint(float particleX, float particleY, out float seekX, out float seekY, out float forceRatio);
    }
}
