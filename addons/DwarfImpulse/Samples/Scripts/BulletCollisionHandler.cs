using DwarfImpulse;
using Godot;

public partial class BulletCollisionHandler : ProjectileCollisionHandler
{
    protected override void OnCollisionEnter()
    {
        var preset = new BounceShake()
                .WithFrequency(50.0f)
                .WithDuration(0.2f)
                .WithJitter(0.2f)
                .WithStartDir(new Vector3(flyDir.X, flyDir.Y, 0))
                .WithOffsetAmount(3, 7)
                .WithEnvelope(new Envelope(10, 1, 0.7f, Degree.Cubic));

        AudioStreamPlayer audioPlayer = new AudioStreamPlayer();
        audioPlayer.Stream = hitSound;
        audioPlayer.VolumeDb = -25;
        GetTree().CurrentScene.AddChild(audioPlayer);
        audioPlayer.Finished += audioPlayer.QueueFree;
        audioPlayer.Play();

        shakeDirector.Shake(preset);
        Owner.QueueFree();
    }
}
