using Godot;

namespace DwarfImpulse
{
    public abstract class ShakePreset
    {
        private float duration = 1.0f;
        private float maxDuration = 1.0f;

        private Envelope envelope = new Envelope();
        public SpatialAttenuation SpatialAttenuation = new SpatialAttenuation();

        public float DurationLeft
        {
            get => duration;
            protected set
            {
                duration = Mathf.Max(0, value);
                MaxDuration = duration;
            }
        }

        public float MaxDuration
        {
            get => maxDuration;
            private set => maxDuration = value;
        }

        public bool IsActive
        {
            get => duration > 0.0f;
        }

        public Envelope Envelope
        {
            get => envelope;
            protected set => envelope = value;
        }


        internal void Update(float delta)
        {
            duration -= delta;
        }

        internal abstract Displacement ExecuteShake(float delta);
    }
}
