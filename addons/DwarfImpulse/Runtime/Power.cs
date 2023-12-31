
namespace DwarfImpulse
{
    public enum Degree { Linear, Quadratic, Cubic };

    internal static class Power
    {
        internal static float Evaluate(float value, Degree degree)
        {
            switch (degree)
            {
                case Degree.Linear:
                    return value;
                case Degree.Quadratic:
                    return value * value;
                case Degree.Cubic:
                    return value * value * value;
                default:
                    return value;
            }
        }
    }
}
