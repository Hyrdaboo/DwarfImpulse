using DwarfImpulse;
using Godot;

public abstract partial class ProjectileCollisionHandler : Node2D
{
    [Export] private float flySpeed = 300.0f;
    [Export] protected AudioStream hitSound;

    protected ShakeDirector2D shakeDirector;
    protected Vector2 flyDir;

    public void Initialize(Vector2 flyDir, ShakeDirector2D shakeDirector)
    {
        this.flyDir = flyDir;
        this.shakeDirector = shakeDirector;
    }

    private void OnBodyEntered(Node2D body)
    {
        OnCollisionEnter();
    }

    protected abstract void OnCollisionEnter();

    public override void _Process(double delta)
    {
        (Owner as Node2D).GlobalPosition += flyDir * flySpeed * (float)delta;
    }
}