using UnityEngine;
using Vexe.Runtime.Types;

namespace Crescendo.API {

    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    public class CharacterJump : CharacterComponent {

        private const float defaultGravity = 9.86f;
        private CharacterGravity _gravity;
        private int _jumpCount;

        [SerializeField]
        private float[] _jumpHeights = {1.5f, 1.5f};

        [Serialize]
        [AnimVar(Filter = ParameterType.Trigger)]
        private int _jumpTrigger;

        public int JumpCount {
            get { return _jumpCount; }
        }

        public int MaxJumpCount {
            get { return _jumpHeights == null ? 0 : _jumpHeights.Length; }
        }

        protected override void Start() {
            base.Start();

            _gravity = GetComponentInChildren<CharacterGravity>();

            Character.OnJump += OnJump;
            Character.OnGrounded += OnGrounded;
            Character.JumpRestrictions += CanJump;
        }

        private bool CanJump() {
            return _jumpCount < _jumpHeights.Length;
        }

        private void OnJump() {
            float g = _gravity == null ? _gravity.Gravity : defaultGravity;

            // Apply upward force to jump
            Vector3 temp = Character.Velocity;
            temp.y = Mathf.Sqrt(2*g*_jumpHeights[_jumpCount]);
            Character.Velocity = temp;

            _jumpCount++;

            // Trigger animation
            Animator.SetTrigger(_jumpTrigger);
        }

        private void OnGrounded() {
            if (Character.IsIsGrounded)
                _jumpCount = 0;
        }

    }

}