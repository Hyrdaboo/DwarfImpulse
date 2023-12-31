using Godot;
using System;

namespace DwarfImpulse
{
    public class PerlinShake : ShakePreset
    {
        private FastNoiseLite noise;
        private Vector3 offsetAmount;
        private Vector3 eulerAnglesAmount;
        private float noiseScrollSpeed = 800;

        private float offset;

        private PerlinShake() { }

        public static PerlinShake CreateWithNoise(FastNoiseLite noise)
        {
            if (noise == null)
                throw new ArgumentNullException("Noise for PerlinShake cannot be Null");

            PerlinShake shake = new PerlinShake();
            shake.noise = noise;
            return shake;
        }

        public PerlinShake WithOffsetAmount(Vector3 offsetAmount)
        {
            this.offsetAmount = offsetAmount;
            return this;
        }

        public PerlinShake WithEulersAmount(Vector3 eulerAnglesAmount)
        {
            this.eulerAnglesAmount = eulerAnglesAmount;
            return this;
        }

        public PerlinShake WithScrollSpeed(float noiseScrollSpeed)
        {
            this.noiseScrollSpeed = noiseScrollSpeed;
            return this;
        }

        public PerlinShake WithDuration(float duration)
        {
            DurationLeft = duration;
            return this;
        }

        public PerlinShake WithEnvelope(Envelope envelope)
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
