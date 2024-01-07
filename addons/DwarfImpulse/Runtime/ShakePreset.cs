using Godot;

namespace DwarfImpulse
{
    /// <summary>
    /// Represents the shake to be applied to the target.
    /// 
    /// <para>
    /// Every shake preset inherits from this base class and implements its
    /// abstract methods. You can use this class to define custom shake presets.
    /// </para>
    /// </summary>
    public abstract class ShakePreset
    {
        private float duration = 1.0f;
        private float maxDuration = 1.0f;

        private Envelope envelope = new Envelope();
        public SpatialAttenuation SpatialAttenuation = new SpatialAttenuation();

        /// <summary>
        /// Gets the amount of time left before this preset stops being active
        /// </summary>
        public float DurationLeft
        {
            get => duration;
            protected set
            {
                duration = Mathf.Max(0, value);
                MaxDuration = duration;
            }
        }

        /// <summary>
        /// Gets the amount of time this preset had upon creation
        /// </summary>
        public float MaxDuration
        {
            get => maxDuration;
            private set => maxDuration = value;
        }

        /// <summary>
        /// If true the shake by this preset is being applied
        /// </summary>
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

        /// <summary>
        /// Every shake preset must implement this method. This method 
        /// is called every frame during a camera shake event.
        /// </summary>
        /// <param name="delta">The time elapsed since the last frame.</param>
        /// <returns>The position and rotation of the target object at a single frame of shake</returns>
        internal abstract Displacement ExecuteShake(float delta);
    }
}
