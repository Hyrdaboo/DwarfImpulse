﻿using Godot;
using System.Collections.Generic;

namespace DwarfImpulse
{
    [GlobalClass]
    public partial class ShakeDirector2D : Node2D
    {
        private Node2D target;
        private float amplitudeOverride = 1.0f;

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

        public void Shake(ShakePreset preset)
        {
            if (target == null) return;
            activeShakes.Add(preset);
        }

        public void TerminateAll()
        {
            activeShakes.Clear();
        }
    }
}