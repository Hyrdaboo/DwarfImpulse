using Godot;
using System;

namespace DwarfImpulse
{
    /// <summary>
    /// Applies shake to the target based on values sampled from noise.
    /// Useful for longer shakes.
    /// <para>
    /// Use the <see cref="CreateWithNoise(FastNoiseLite)"/> method to
    /// create a new instance of this preset.
    /// </para>
    /// </summary>
    public class NoiseShake : ShakePreset
    {
        private FastNoiseLite noise;
        private Vector3 offsetAmount;
        private Vector3 eulerAnglesAmount;
        private float noiseScrollSpeed = 800;

        private float offset;

        private NoiseShake() { }

        /// <summary>
        /// Creates a new instance of this preset and initializes its noise
        /// </summary>
        public static NoiseShake CreateWithNoise(FastNoiseLite noise)
        {
            if (noise == null)
                throw new ArgumentNullException("Noise for PerlinShake cannot be Null");

            NoiseShake shake = new NoiseShake();
            shake.noise = noise;
            return shake;
        }

        /// <summary>
        /// Sets the amount of positional offset applied to target.
        /// </summary>
        public NoiseShake WithOffsetAmount(Vector3 offsetAmount)
        {
            this.offsetAmount = offsetAmount;
            return this;
        }

        /// <summary>
        /// Sets the amount of angular offset applied to target.
        /// </summary>
        public NoiseShake WithEulersAmount(Vector3 eulerAnglesAmount)
        {
            this.eulerAnglesAmount = eulerAnglesAmount;
            return this;
        }

        /// <summary>
        /// Sets the speed at which the noise is being scrolled, effectively acting as additional frequency.
        /// </summary>
        /// <param name="noiseScrollSpeed">The scroll speed.</param>
        public NoiseShake WithScrollSpeed(float noiseScrollSpeed)
        {
            this.noiseScrollSpeed = noiseScrollSpeed;
            return this;
        }

        public NoiseShake WithDuration(float duration)
        {
            DurationLeft = duration;
            return this;
        }

        public NoiseShake WithEnvelope(Envelope envelope)
        {
            Envelope = envelope;
            return this;
        }

        internal override Displacement ExecuteShake(float delta)
        {
            offset += delta * noiseScrollSpeed;

            Displacement disp = Displacement.Zero;

            int prevSeed = noise.Seed;

            if (offsetAmount.Length() != 0)
            {
                disp.Offset.X = noise.GetNoise1D(offset) * offsetAmount.X;
                noise.Seed++;
                disp.Offset.Y = noise.GetNoise1D(offset) * offsetAmount.Y;
                noise.Seed++;
                disp.Offset.Z = noise.GetNoise1D(offset) * offsetAmount.Z;
                noise.Seed++;
            }

            if (eulerAnglesAmount.Length() != 0)
            {
                disp.EulerAngles.X = noise.GetNoise1D(offset) * eulerAnglesAmount.X;
                noise.Seed++;
                disp.EulerAngles.Y = noise.GetNoise1D(offset) * eulerAnglesAmount.Y;
                noise.Seed++;
                disp.EulerAngles.Z = noise.GetNoise1D(offset) * eulerAnglesAmount.Z;
            }

            noise.Seed = prevSeed;

            return disp;
        }
    }
}
