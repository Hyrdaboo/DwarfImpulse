using Godot;

namespace DwarfImpulse
{
    /// <summary>
    /// Represents the positional and angular displacement applied to the target object.
    /// </summary>
    public struct Displacement
    {
        public Vector3 Offset;
        public Vector3 EulerAngles;

        public Displacement(Vector3 offset, Vector3 eulerAngles)
        {
            Offset = offset;
            EulerAngles = eulerAngles;
        }

        public static Displacement Zero
        {
            get => new Displacement(Vector3.Zero, Vector3.Zero);
        }

        public static Displacement operator +(Displacement left, Displacement right)
        {
            return new Displacement(left.Offset + right.Offset, left.EulerAngles + right.EulerAngles);
        }

        public static Displacement operator -(Displacement left, Displacement right)
        {
            return new Displacement(left.Offset - right.Offset, left.EulerAngles - right.EulerAngles);
        }

        public static Displacement operator *(Displacement disp, float value)
        {
            return new Displacement(disp.Offset * value, disp.EulerAngles * value);
        }

        public override string ToString()
        {
            return $"{Offset}\n {EulerAngles}\n ____________________________________________________";
        }
    }
}
