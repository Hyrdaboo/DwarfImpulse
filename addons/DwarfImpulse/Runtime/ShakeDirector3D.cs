using Godot;
using System.Collections.Generic;

namespace DwarfImpulse
{
    /// <summary>
    /// Applies shakes to the target object in 3D.
    /// 
    /// <para>When the <see cref="ShakeDirector3D.Shake(ShakePreset)"/> method is called, 
    /// it stores the shake in the list of active shakes and applies 
    /// displacement to the target object until the specified time runs 
    /// out, at which point the shake preset is automatically cleared.
    /// Additionally, <see cref="Envelope"/> and <see cref="SpatialAttenuation"/>
    /// are applied to the output Displacement of all shakes.
    /// </para>
    /// </summary>
    [GlobalClass]
    public partial class ShakeDirector3D : Node3D
    {
        private Node3D target;
        private float amplitudeOverride = 1.0f;

        /// <summary>
        /// The object targeted for shaking.
        /// <para>
        /// <strong>Note:</strong> You can't change the target while there are 
        /// active shakes running.
        /// </para>
        /// </summary>
        [Export]
        public Node3D Target
        {
            get => target;
            set
            {   
                if (activeShakes.Count > 0)
                    GD.PrintErr("Cannot change target while there are active shakes running");
                else
                    target = value;
            }
        }

        /// <summary>
        /// Scales all shakes by the specified amount. 
        /// <para>
        /// Setting this to, for example, 2 means all shakes will be twice as strong,
        /// and 0 means no shake will take place. Negative values are ignored
        /// </para>
        /// </summary>
        [Export]
        public float AmplitudeOverride
        {
            get => amplitudeOverride;
            set => amplitudeOverride = Mathf.Max(0, value);
        }

        private List<ShakePreset> activeShakes = new List<ShakePreset>();

        public override void _Ready()
        {
            CallDeferred(nameof(AfterReady));
        }

        private void AfterReady()
        {
            Node currentScene = GetTree().CurrentScene;
            Reparent(currentScene);
            currentScene.MoveChild(this, currentScene.GetChildCount() - 1);
        }

        Displacement displacementLastFrame;
        Displacement originalOrientation;
        public override void _Process(double delta)
        {
            if (!IsInstanceValid(target)) return;
            // record the original orientation by subtracting any displacement done last frame
            originalOrientation = new Displacement(target.Position, target.Rotation) - displacementLastFrame;
            
            float fDelta = (float)delta;

            Displacement disp = Displacement.Zero;
            for (int i = 0; i < activeShakes.Count; i++)
            {
                ShakePreset shake = activeShakes[i];
                shake.Update(fDelta);
                if (!shake.IsActive)
                {
                    activeShakes.RemoveAt(i);
                    continue;
                }

                float shakeNormalizedTime = 1.0f - shake.DurationLeft / shake.MaxDuration;
                disp += shake.ExecuteShake(fDelta) * shake.Envelope.Evaluate(shakeNormalizedTime) * shake.SpatialAttenuation.Evaluate(target.GlobalPosition) * amplitudeOverride;
            }

            disp.Offset = Quaternion.FromEuler(target.Rotation) * disp.Offset;
            Displacement combined = disp + originalOrientation;

            target.Position = combined.Offset;
            target.Rotation = combined.EulerAngles;

            displacementLastFrame = disp;
        }

        /// <summary>
        /// Starts a new shake event.
        /// </summary>
        /// <param name="preset">The shake preset to be used for shaking.</param>
        public void Shake(ShakePreset preset)
        {
            if (target == null) return;
            activeShakes.Add(preset);
        }

        /// <summary>
        /// Terminates all currently active shakes
        /// </summary>
        public void TerminateAll()
        {
            activeShakes.Clear();
        }
    }
}
