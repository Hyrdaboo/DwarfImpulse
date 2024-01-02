using Godot;

namespace DwarfImpulse
{
    /// <summary>
    /// Controls the strength of all shakes over distance in 3D.
    /// Used by the <see cref="ShakeDirector3D"/>
    /// </summary>
    public struct SpatialAttenuation
    {
        private float clippingDistance = 2.0f;
        private float falloffScale = 3.0f;
        private Degree degree = Degree.Quadratic;
        private Vector3 shakeSource = new Vector3(float.NaN, float.NaN, 0);

        public SpatialAttenuation() {}

        /// <summary>
        /// The minimum distance beyond which the amplitude of the shake is at its maximum.
        /// </summary>
        public float ShakeSourceMinDistance
        {
            get => clippingDistance;
            set => clippingDistance = Mathf.Max(0, value);
        }

        /// <summary>
        /// The maximum distance beyond which the amplitude of the shake reaches 0.
        /// </summary>
        public float ShakeSourceMaxDistance
        {
            get => clippingDistance + falloffScale;
            set => falloffScale = Mathf.Max(0.001f, value - clippingDistance);
        }

        /// <summary>
        /// The degree to which the attenuation function is raised. Higher values result in a smoother output.
        /// </summary>
        public Degree Degree
        {
            get => degree;
            set => degree = value;
        }

        /// <summary>
        /// Represents the 3D position in space from which the shake is assumed to originate.
        /// </summary>
        public Vector3 ShakeSource
        {
            get => shakeSource;
            set => shakeSource = value;
        }

        /// <summary>
        /// Evaluates the amplitude of the attenuation function at a given point.
        /// Used internally by the <see cref="ShakeDirector3D"/>.
        /// </summary>
        /// <param name="observer">The 3D location of the observer, i.e., the target object experiencing the shake.</param>
        /// <returns>The amplitude of the attenuation function at the specified location.</returns>
        public float Evaluate(Vector3 observer)
        {
            if (float.IsNaN(shakeSource.X))
                return 1;
            float x = observer.DistanceTo(shakeSource);
            return Power.Evaluate(Mathf.Max(0, Mathf.Min(1, 1 - (x - clippingDistance) / falloffScale)), degree);
        }
    }
}