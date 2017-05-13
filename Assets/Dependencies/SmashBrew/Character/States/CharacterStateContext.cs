using Newtonsoft.Json;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Characters {

    public struct ButtonContext {

        public bool LastFrame;
        public bool Current;

        public bool WasPressed {
            get { return !LastFrame && Current; }
        }

        public bool WasReleased {
            get { return LastFrame && !Current; }
        }

        public void Update(bool value) {
            LastFrame = Current;
            Current = value;
        }

        public override int GetHashCode() {
            return 2 * LastFrame.GetHashCode() + Current.GetHashCode();
        }

    }

    public struct InputContext {
        public Vector2 Movement;
        public Vector2 Smash;
        public ButtonContext Attack;
        public ButtonContext Special;
        public ButtonContext Jump;
        public ButtonContext Shield;

        public override int GetHashCode() {
            return Shield.GetHashCode() + 
                3 * Jump.GetHashCode() +
                7 * Special.GetHashCode() +
                11 * Attack.GetHashCode() +
                17 * Smash.GetHashCode() +
                19 * Movement.GetHashCode();
        }

        public override string ToString() {
            return GetHashCode().ToString();
        }
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

        public override string ToString() {
            return "t:{0} d:{5} g:{1} l:{2} h:{3} s:{4} i:{6}".With(
                NormalizedAnimationTime, IsGrounded, IsGrabbingLedge,
                IsHit, ShieldHP, Direction, Input
            );
        }

    }

}

