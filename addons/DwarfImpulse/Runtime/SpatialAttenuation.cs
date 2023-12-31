using Godot;

namespace DwarfImpulse
{
    public struct SpatialAttenuation
    {
        private float clippingDistance = 2.0f;
        private float falloffScale = 3.0f;
        private Degree degree = Degree.Quadratic;
        private Vector3 shakeSource = new Vector3(float.NaN, float.NaN, 0);

        public SpatialAttenuation() {}

        public float ShakeSourceMinDistance
        {
            get => clippingDistance;
            set => clippingDistance = Mathf.Max(0, value);
        }

        public float ShakeSourceMaxDistance
        {
            get => clippingDistance + falloffScale;
            set => falloffScale = Mathf.Max(0.001f, value - clippingDistance);
        }

        public Degree Degree
        {
            get => degree;
            set => degree = value;
        }

        public Vector3 ShakeSource
        {
            get => shakeSource;
            set => shakeSource = value;
        }

        public float Evaluate(Vector3 observer)
        {
            if (float.IsNaN(shakeSource.X))
                return 1;
            float x = observer.DistanceTo(shakeSource);
            return Power.Evaluate(Mathf.Max(0, Mathf.Min(1, 1 - (x - clippingDistance) / falloffScale)), degree);
        }
    }
}