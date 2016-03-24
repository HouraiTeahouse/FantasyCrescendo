using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    /// <summary>
    /// Constants for fast access to Character's animator parameters
    /// </summary>
    public static class CharacterAnim {
        // Input
        public static readonly int HorizontalInput = Animator.StringToHash("horizontal input");
        public static readonly int VerticalInput = Animator.StringToHash("vertical input");
        public static readonly int AttackInput = Animator.StringToHash("attack");
        public static readonly int SpecialInput = Animator.StringToHash("special");
        public static readonly int Jump = Animator.StringToHash("jump");
        public static readonly int ShieldInput = Animator.StringToHash("shield");

        // State Variables
        public static readonly int Grounded = Animator.StringToHash("grounded");
        public static readonly int ShieldHP = Animator.StringToHash("shieldHealth");

        public static readonly int Combo = Animator.StringToHash("combo");

        // States
        public static readonly int Run = Animator.StringToHash("run");

        public static bool IsInState(this Animator animator, int stateHash) {
            return animator && animator.GetCurrentAnimatorStateInfo(0).shortNameHash == stateHash;
        }

    }
}
