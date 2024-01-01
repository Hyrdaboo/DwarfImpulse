using Godot;

public partial class TitanController : Node3D
{
    [Export] private float speed = 1.0f;

    public override void _Process(double delta)
    {
        GlobalPosition += Basis.Z * (float)delta * speed;
    }
}
