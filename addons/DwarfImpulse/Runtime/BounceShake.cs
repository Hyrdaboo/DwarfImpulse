using Godot;

namespace DwarfImpulse
{
    /// <summary>
    /// Applies shake to the target based on a directional vector. Useful 
    /// for shorter, more precise shakes.
    /// <para>
    /// It works by moving back and forth along a predefined vector and 
    /// introduces a bit of randomness to that vector based on a jitter variable.
    /// The directional vector is by default <see cref="Vector3.Right"/>
    /// and can be set with the <see cref="WithStartDir(Vector3)"/> method upon creation
    /// </para>
    /// </summary>
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

        /// <summary>
        /// Sets the amount of positional offset applied to target.
        /// </summary>
        public BounceShake WithOffsetAmount(float minOffset, float maxOffset)
        {
            minOffsetAmount = Mathf.Max(0, minOffset);
            maxOffsetAmount = Mathf.Max(0, maxOffset);
            return this;
        }

        /// <summary>
        /// Sets the amount of angular offset applied to target.
        /// </summary>
        public BounceShake WithEulersAmount(float minEulers, float maxEulers)
        {
            minEulersAmount = Mathf.Max(0, minEulers);
            maxEulersAmount = Mathf.Max(0, maxEulers);
            return this;
        }

        /// <summary>
        /// Sets the frequency of the shake, determining how fast the target object moves
        /// along the vector during a given time.
        /// </summary>
        public BounceShake WithFrequency(float frequency)
        {
            this.frequency = Mathf.Max(1.0f, frequency);
            return this;
        }

        /// <summary>
        /// Sets the jitter value, introducing randomness to the shake direction.
        /// </summary>
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

        /// <summary>
        /// Sets the vector along which the shake takes place. 
        /// </summary>
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
    }
}
