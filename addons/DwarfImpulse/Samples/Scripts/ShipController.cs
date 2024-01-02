using DwarfImpulse;
using Godot;
using Godot.Collections;

public partial class ShipController : CharacterBody2D
{
	[Export] private float speed = 300.0f;
	[Export] private float stopSpeed = 20.0f;
	[Export] private ShakeDirector2D shakeDirector;
	[ExportCategory("Projectile Handling")]
	[ExportGroup("Bullet")]
	[Export] private PackedScene bullet;
	[Export] private Array<Node2D> bulletMuzzles;
	[Export] private float bulletFireRate = 0.35f;
	[Export] private AudioStreamPlayer shootSfx;
	[ExportGroup("Bomb")]
	[Export] private PackedScene bomb;
	[Export] private Node2D bombMuzzle;
	[Export] private float bombFireRate = 1.0f;

	private Timer bombTimer;

    public override void _Ready()
    {
		Timer bulletTimer = new Timer();
		bulletTimer.WaitTime = bulletFireRate;
		bulletTimer.OneShot = false;
		bulletTimer.Timeout += fireBullet;
		AddChild(bulletTimer);
		bulletTimer.Start();

		bombTimer = new Timer();
		bombTimer.WaitTime = bombFireRate;
		bombTimer.OneShot = true;
		AddChild(bombTimer);
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		Vector2 direction = Input.GetVector("Left", "Right", "Up", "Down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * speed;
			velocity.Y = direction.Y * speed;
		}
		else
		{
			velocity = velocity.MoveToward(Vector2.Zero, (float)delta * stopSpeed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

    public override void _Process(double delta)
    {
		LookAt(GetGlobalMousePosition());

		if (Input.IsActionJustPressed("Jump") && bombTimer.IsStopped())
		{
			bombTimer.Start();
			instantiateProjectile(bomb, bombMuzzle);
		}
    }

	private void fireBullet()
	{
        if (Input.IsActionPressed("Fire1"))
        {
            shootSfx.Play();
            foreach (var muzzle in bulletMuzzles)
            {
				instantiateProjectile(bullet, muzzle);
            }
        }
    }

	private void instantiateProjectile(PackedScene proj, Node2D spawnPos)
	{
        var projectile = proj.Instantiate(GetTree().CurrentScene, spawnPos.GlobalPosition).FindChildOfType<ProjectileCollisionHandler>() as ProjectileCollisionHandler;
        projectile?.Initialize(Transform.X.Normalized(), shakeDirector);
    }
}
