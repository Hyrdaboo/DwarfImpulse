using Godot;
using System.Collections.Generic;

namespace DwarfImpulse
{
    /// <summary>
    /// Applies shakes to the target object in 2D.
    /// 
    /// <para>When the <see cref="ShakeDirector3D.Shake(ShakePreset)"/> method is called, 
    /// it stores the shake in the list of active shakes and applies 
    /// displacement to the target object until the specified time runs 
    /// out, at which point the shake preset is automatically cleared.
    /// Additionally, <see cref="Envelope"/> is applied to the output
    /// Displacement of all shakes.
    /// </para>
    /// </summary>
    [GlobalClass]
    public partial class ShakeDirector2D : Node2D
    {
        private Node2D target;
        private float amplitudeOverride = 1.0f;
        private bool isGlobal = false;

        public ShakeDirector2D() { }

        /// <summary>
        /// Setting the optional param isGlobal to true will make this node
        /// the child of the root node instead of the current scene. If your target
        /// is local to the current scene and you want to make ShakeDirector global
        /// make sure you set the target every time you change or reload scenes or else the target
        /// will be invalid.
        /// </summary>
        public ShakeDirector2D(bool isGlobal)
        {
            this.isGlobal = isGlobal;
        }

        /// <summary>
        /// The object targeted for shaking.
        /// <para>
        /// <strong>Note:</strong> You can't change the target while there are 
        /// active shakes running.
        /// </para>
        /// </summary>
        [Export]
        public Node2D Target
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
            Node currentScene = isGlobal ? GetTree().Root : GetTree().CurrentScene;
            Reparent(currentScene);
            currentScene.MoveChild(this, currentScene.GetChildCount() - 1);
            currentScene.ChildEnteredTree += (node) => currentScene.MoveChild(this, currentScene.GetChildCount() - 1);
        }

        Displacement displacementLastFrame;
        Displacement originalOrientation;
        public override void _Process(double delta)
        {
            if (!IsInstanceValid(target)) return;
            // record the original orientation by subtracting any displacement done last frame
            originalOrientation = new Displacement(
                new Vector3 { X = target.Position.X, Y = target.Position.Y}, 
                new Vector3 { Z = target.Rotation }) - displacementLastFrame;

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
                disp += shake.ExecuteShake(fDelta) * shake.Envelope.Evaluate(shakeNormalizedTime) * amplitudeOverride;
            }

            disp.Offset = disp.Offset.Rotated(Vector3.Back, originalOrientation.EulerAngles.Z);
            Displacement combined = disp + originalOrientation;

            target.Position = new Vector2(combined.Offset.X, combined.Offset.Y);
            target.Rotation = combined.EulerAngles.Z;

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
