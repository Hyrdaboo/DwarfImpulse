using Godot;

namespace DwarfImpulse
{
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

        public float Attack
        {
            get => attack;
            set => attack = Mathf.Max(0.1f, value);
        }

        public float Sustain
        {
            get => sustain;
            set => sustain = Mathf.Max(0, value);
        }

        public float Decay
        {
            get => decay;
            set => decay = Mathf.Max(0.1f, value);
        }

        public Degree EnvelopeDegree
        {
            get => degree;
            set => degree = value;
        }

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