using DwarfImpulse;
using Godot;

public partial class BombCollisionHandler : ProjectileCollisionHandler
{
    [Export] private FastNoiseLite noise;

    protected override void OnCollisionEnter()
    {
        var preset = NoiseShake.CreateWithNoise(noise)
                .WithOffsetAmount(new Vector3(25, 25, 0))
                .WithEnvelope(new Envelope(10, 1, 0.6f, Degree.Quadratic))
                .WithDuration(1.0f);

        AudioStreamPlayer audioPlayer = new AudioStreamPlayer();
        audioPlayer.Stream = hitSound;
        GetTree().CurrentScene.AddChild(audioPlayer);
        audioPlayer.Finished += audioPlayer.QueueFree;
        audioPlayer.Play();

        shakeDirector.Shake(preset);
        Owner.QueueFree();
    }
}
