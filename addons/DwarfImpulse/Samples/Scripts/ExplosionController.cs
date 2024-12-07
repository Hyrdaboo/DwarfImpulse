using DwarfImpulse;
using Godot;
using Godot.Collections;

public partial class ExplosionController : GpuParticles3D
{
    [Export]
	private ShakeDirector3D shakeDirector;
    [Export]
    private OmniLight3D explosionLight;
    [Export]
    private AudioStreamPlayer3D audioPlayer;
    [Export]
    private Array<RigidBody3D> bodies;
    [Export] private float forceStrength = 10.0f;
    [ExportCategory("Camera Shake")]
    [Export]
    private FastNoiseLite noise;
    [Export]
    private Vector3 shakeRotationAmount = new Vector3(0.025f, 0, 0.04f);
    [ExportGroup("Envelope")]
    [Export]
    private float attack = 10.0f;
    [Export]
    private float sustain = 1.0f;
    [Export]
    private float decay = 0.5f;

    private float energyAtStart;
    private bool canFire = true;
    
    public override void _Ready()
    {
        energyAtStart = explosionLight.LightEnergy;
        Finished += OnFinish;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Jump") && canFire)
        {
            canFire = false;
            shakeDirector.Shake(NoiseShake.CreateWithNoise(noise)
                .WithDuration((float)Lifetime)
                .WithScrollSpeed(1200) //the speed at which the noisemap is scrolled (basically frequency)
                .WithEnvelope(new Envelope(attack, sustain, decay, Degree.Quadratic))
                .WithEulersAmount(shakeRotationAmount), () => GD.Print("Done!"));
            Restart();

            explosionLight.Visible = true;
            explosionLight.LightEnergy = energyAtStart;
            audioPlayer.Play();
            foreach (var body in bodies)
            {
                Vector3 v = (body.GlobalPosition - GlobalPosition);
                body.ApplyCentralImpulse(v.Normalized() * forceStrength / v.Length());
            }


            Tween tween = GetTree().CreateTween();
            tween.TweenProperty(explosionLight, "light_energy", 0, Lifetime);
        }
    }

    private void OnFinish()
    {
        canFire = true;
    }
}
