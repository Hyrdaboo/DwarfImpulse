using DwarfImpulse;
using Godot;

public partial class FootstepShake : Node3D
{
    [Export] private float rayDistance = 100f;
    [Export] private float minDistance = 10;
    [Export] private float maxDistance = 50;
    [ExportCategory("Camera Shake")]
    [Export] private FastNoiseLite noise;
    [Export] private Vector3 shakeRotationAmount = new Vector3(0.025f, 0, 0.04f);
    [Export] private Vector3 shakeOffsetAmount = new Vector3(0.1f, 0.1f, 0);
    private RayCast3D raycast;
    private AudioStreamPlayer3D audioPlayer;
    private ShakeDirector3D shakeDirector;

    public override void _Ready()
    {
        audioPlayer = this.FindChildOfType<AudioStreamPlayer3D>() as AudioStreamPlayer3D;
        CallDeferred(nameof(AfterReady));
    }

    private void AfterReady()
    {
        raycast = new RayCast3D();
        raycast.Scale = Scale;
        Owner.AddChild(raycast);
        raycast.TargetPosition = Vector3.Down * rayDistance;
        shakeDirector = GetTree().CurrentScene.FindChildOfType<ShakeDirector3D>() as ShakeDirector3D;
    }

    private bool onGroundLastFrame = true;
    public override void _Process(double delta)
    {
        raycast.GlobalPosition = GlobalPosition;


        bool onGround = raycast.GetCollider() != null;

        if (onGround && !onGroundLastFrame)
        {
            var preset = NoiseShake.CreateWithNoise(noise)
                .WithDuration(1.0f)
                .WithScrollSpeed(1000) //the speed at which the noisemap is scrolled (basically frequency)
                .WithEnvelope(new Envelope(10.0f, 2.0f, 0.35f, Degree.Quadratic))
                .WithEulersAmount(shakeRotationAmount)
                .WithOffsetAmount(shakeOffsetAmount);
            preset.SpatialAttenuation.ShakeSourceMinDistance = minDistance;
            preset.SpatialAttenuation.ShakeSourceMaxDistance = maxDistance;
            preset.SpatialAttenuation.ShakeSource = GlobalPosition;

            shakeDirector?.Shake(preset);
            audioPlayer.Play();
            //GD.Print("STOMP");
        }

        onGroundLastFrame = onGround;
    }
}
