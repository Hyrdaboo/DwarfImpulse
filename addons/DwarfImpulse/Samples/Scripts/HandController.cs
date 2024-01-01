using DwarfImpulse;
using Godot;
using System.Threading.Tasks;

public partial class HandController : Node3D
{
    [Export] private ShakeDirector3D shakeDirector;
    [Export] private AudioStreamPlayer audioPlayer;
    [Export] private Vector3 shakeDir = Vector3.Left;
    private AnimationPlayer animPlayer;

    public override void _Ready()
    {
        animPlayer = this.FindChildOfType<AnimationPlayer>() as AnimationPlayer;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Fire1"))
        {
            if (!string.IsNullOrEmpty(animPlayer.CurrentAnimation)) 
                return;

            float scale = (int)(GD.Randi() % 2 * 2 - 1);
            Scale = new Vector3(scale, 1, 1);
            shakeDir.X = -scale;

            animPlayer.CurrentAnimation = "Swing_Left";
            animPlayer.SpeedScale = 1.5f;

            ShakeCam();
        }
    }

    private async void ShakeCam()
    {
        await Task.Delay(300);
        var preset = new BounceShake()
                .WithFrequency(30.0f)
                .WithDuration(0.2f)
                .WithJitter(0.2f)
                .WithStartDir(shakeDir)
                .WithOffsetAmount(0.05f, 0.13f)
                .WithEulersAmount(0.01f, 0.04f)
                .WithEnvelope(new Envelope(10, 1, 0.7f, Degree.Quadratic));
        audioPlayer.Play();
        audioPlayer.PitchScale = Mathf.Lerp(1.0f, 1.2f, GD.Randf());

        shakeDirector.Shake(preset);
    }
}
