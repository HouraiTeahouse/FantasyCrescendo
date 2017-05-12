using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Characters {

    public struct ButtonContext {

        public bool LastFrame;
        public bool Currrent;

        public bool WasPressed {
            get { return !LastFrame && Currrent; }
        }

        public bool WasReleased {
            get { return LastFrame && !Currrent; }
        }

        public void Update(bool value) {
            LastFrame = Currrent;
            Currrent = value;
        }

    }

    public struct InputContext {
        public Vector2 Movement;
        public Vector2 Smash;
        public ButtonContext Attack;
        public ButtonContext Special;
        public ButtonContext Jump;
        public ButtonContext Shield;
    }

    public class CharacterStateContext {
        public float NormalizedAnimationTime { get; set; }
        public bool IsGrounded { get; set; }
        public bool IsGrabbingLedge { get; set; }
        public bool IsHit { get; set; }
        public float ShieldHP { get; set; }
        // The direction the character is facing in.
        // Will be positive if facing to the right.
        // Will be negative if facing to the left.
        public float Direction { get; set; }
        public InputContext Input { get; set; }
    }

}

