using Godot;

namespace DwarfImpulse
{
    /// <summary>
    /// Controls the strength of all shakes over time.
    /// </summary>
    public struct Envelope
    {
        private float attack = 1.0f;
        private float sustain = 1.0f;
        private float decay = 1.0f;
        private Degree degree = Degree.Quadratic;

        public Envelope() {}
        public Envelope(float attack, float sustain, float decay, Degree degree)
        {
            this.attack = attack;
            this.sustain = sustain;
            this.decay = decay;
            this.degree = degree;
        }

        /// <summary>
        /// How fast the amplitude raises during a shake event.
        /// </summary>
        public float Attack
        {
            get => attack;
            set => attack = Mathf.Max(0.1f, value);
        }

        /// <summary>
        /// The duration for which the amplitude stays at its maximum value during a shake event.
        /// </summary>
        public float Sustain
        {
            get => sustain;
            set => sustain = Mathf.Max(0, value);
        }

        /// <summary>
        /// How fast the amplitude falls during a shake event.
        /// </summary>
        public float Decay
        {
            get => decay;
            set => decay = Mathf.Max(0.1f, value);
        }

        /// <summary>
        /// The degree to which the envelope function is raised. Higher values result in a smoother output.
        /// </summary>
        public Degree EnvelopeDegree
        {
            get => degree;
            set => degree = value;
        }

        /// <summary>
        /// Evaluates the amplitude of the envelope function at a given point.
        /// Used internally by the <see cref="ShakeDirector3D"/> and <see cref="ShakeDirector2D"/>.
        /// </summary>
        /// <param name="normalizedTime">Normalized time between 0 and 1.</param>
        /// <returns>The amplitude of the envelope function at the specified normalized time.</returns>
        public float Evaluate(float normalizedTime)
        {
            normalizedTime = Mathf.Clamp(normalizedTime, 0.0f, 1.0f);
            float xBound = 1.0f / attack;
            
            float envelopeLimit = xBound + Sustain + 1.0f / decay;
            float x = normalizedTime * envelopeLimit;

            if (x > 0 && x < xBound)
                return Power.Evaluate(attack * x, degree);
            if (x > xBound && x < xBound + sustain)
                return 1.0f;
            if (x > xBound + sustain && x < envelopeLimit)
                return Power.Evaluate(1.0f - (x - xBound - sustain) * decay, degree);
            return 0;
        }
    }
}