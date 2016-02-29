using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    /// <summary>
    /// Constants for fast access to Character's animator parameters
    /// </summary>
    public static class CharacterAnimVars {

        // Input
        public static readonly int HorizontalInput = Animator.StringToHash("horizontal input");
        public static readonly int VerticalInput = Animator.StringToHash("vertical input");
        public static readonly int AttackInput = Animator.StringToHash("attack");
        public static readonly int SpecialInput = Animator.StringToHash("special");
        public static readonly int JumpInput = Animator.StringToHash("jump");
        public static readonly int ShieldInput = Animator.StringToHash("shield");

        // Flags
        public static readonly int CanJump = Animator.StringToHash("canJump");

        // State Variables
        public static readonly int Grounded = Animator.StringToHash("grounded");
        public static readonly int ShieldHP = Animator.StringToHash("shieldHealth");
    }

}
