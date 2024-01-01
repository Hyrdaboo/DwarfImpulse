using Godot;

public partial class PlayerController : CharacterBody3D
{
	[Export]
	private float Speed = 5.0f;
	[Export]
	private float JumpVelocity = 4.5f;
	[Export]
	private float mouseSensitivity = 3.0f;

	private Camera3D camera;

	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _Ready()
    {
		Input.MouseMode = Input.MouseModeEnum.Captured;
		camera = this.FindChildOfType<Camera3D>() as Camera3D;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotionEvt)
		{
			Vector2 mouseDelta = -mouseMotionEvt.Relative / GetViewport().GetWindow().Size;

			Rotate(Vector3.Up, mouseDelta.X * mouseSensitivity);
			camera.RotateX(mouseDelta.Y * mouseSensitivity);
            camera.Rotation = camera.Rotation.Clamp(Vector3.Left * Mathf.DegToRad(90), Vector3.Right * Mathf.DegToRad(90));
		}

		if (@event is InputEventKey keyEvt)
		{
			if (keyEvt.Pressed && !keyEvt.IsEcho() && keyEvt.Keycode == Key.Escape)
			{
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}
		}

		if (@event is InputEventMouseButton mouseEvt)
		{
			if (mouseEvt.Pressed && !mouseEvt.IsEcho() && mouseEvt.ButtonIndex == MouseButton.Left)
			{
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }
		}
    }

	Vector3 straightVelocity;
    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		if (Input.IsActionJustPressed("Jump") && IsOnFloor())
			velocity.Y = JumpVelocity;

		Vector2 inputDir = Input.GetVector("Left", "Right", "Up", "Down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = 0;
			velocity.Z = 0;
		}

		straightVelocity = straightVelocity.MoveToward(new Vector3(velocity.X, 0, velocity.Z), (float)delta * 10);
		Velocity = new Vector3(straightVelocity.X, velocity.Y, straightVelocity.Z);
		MoveAndSlide();
	}
}
