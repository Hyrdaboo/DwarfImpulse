using Godot;

namespace DwarfImpulse
{
    internal class BounceShake : ShakePreset
    {
        private Vector3 startDir = Vector3.Right;
        private float minOffsetAmount = 0.5f;
        private float maxOffsetAmount = 1.0f;
        private float minEulersAmount = 0.05f;
        private float maxEulersAmount = 0.2f;
        private float frequency = 1.0f;
        private float jitter = 0.5f;

        public BounceShake()
        {
            WithEnvelope(new Envelope(1000.0f, 10.0f, 1000.0f, Degree.Linear));
        }

        public BounceShake WithOffsetAmount(float minOffset, float maxOffset)
        {
            minOffsetAmount = Mathf.Max(0, minOffset);
            maxOffsetAmount = Mathf.Max(0, maxOffset);
            return this;
        }

        public BounceShake WithEulersAmount(float minEulers, float maxEulers)
        {
            minEulersAmount = Mathf.Max(0, minEulers);
            maxEulersAmount = Mathf.Max(0, maxEulers);
            return this;
        }

        public BounceShake WithFrequency(float frequency)
        {
            this.frequency = Mathf.Max(1.0f, frequency);
            return this;
        }

        public BounceShake WithJitter(float jitter)
        {
            this.jitter = Mathf.Max(0, jitter);
            return this;
        }

        public BounceShake WithDuration(float duration)
        {
            DurationLeft = duration;
            return this;
        }

        public BounceShake WithEnvelope(Envelope envelope)
        {
            Envelope = envelope;
            return this;
        }

        public BounceShake WithStartDir(Vector3 startDir)
        {
            this.startDir = startDir.Normalized();
            return this;
        }

        float offset;
        float pivotLastFrame;
        Vector3 currentDir;
        float currentOffsetAmount, currentEulersAmount;
        internal override Displacement ExecuteShake(float delta)
        {
            offset += delta * frequency;

            float pivot = Mathf.Sin(offset);
            
            if (Mathf.Sign(pivot) != Mathf.Sign(pivotLastFrame))
            {
                GD.Randomize();
                float r = (GD.Randf() * 2 - 1) * jitter;
                currentDir = startDir.Rotated(Vector3.Forward, r).Normalized();
                currentOffsetAmount = (float)GD.RandRange(minOffsetAmount, maxOffsetAmount);
                currentEulersAmount = (float)GD.RandRange(minEulersAmount, maxEulersAmount);
            }

            pivotLastFrame = pivot;

            Displacement disp = new Displacement(currentDir * currentOffsetAmount * pivot, Vector3.Zero);
            disp.EulerAngles = new Basis(Vector3.Forward.Cross(currentDir), pivot * currentEulersAmount).GetEuler();
            return disp;
        }

        public static Vector3 randomDir()
        {
            GD.Randomize();
            float x = GD.Randf() * 2 - 1;
            float y = GD.Randf() * 2 - 1;
            float z = GD.Randf() * 2 - 1;
            return new Vector3(x, y, z).Normalized();
        }
    }
}
