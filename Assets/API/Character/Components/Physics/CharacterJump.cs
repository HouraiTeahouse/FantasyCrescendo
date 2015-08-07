using UnityEngine;

namespace Crescendo.API {

    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    public class CharacterJump : CharacterComponent {

        private const float defaultGravity = 9.86f;
        private CharacterGravity _gravity;
        private int _jumpCount;

        [SerializeField]
        private AnimationTrigger animationTrigger = new AnimationTrigger("jump");

        [SerializeField]
        private float[] _jumpHeights = {1.5f, 1.5f};

        public int JumpCount {
            get { return _jumpCount; }
        }

        public int MaxJumpCount {
            get { return _jumpHeights == null ? 0 : _jumpHeights.Length; }
        }

        protected override void Start() {
            base.Start();

            _gravity = GetComponentInChildren<CharacterGravity>();
            animationTrigger.Animator = Character.Animator;
            
            Character.OnJump += OnJump;
            Character.OnGrounded += OnGrounded;
            Character.JumpRestrictions += CanJump;
        }

        private bool CanJump() {
            return _jumpCount < _jumpHeights.Length;
        }

        private void OnJump() {
            var g = _gravity == null ? _gravity.Gravity : defaultGravity;

            // Apply upward force to jump
            Vector3 temp = Character.Velocity;
            temp.y = Mathf.Sqrt(2*g*_jumpHeights[_jumpCount]);
            Character.Velocity = temp;

            _jumpCount++;

            // Trigger animation
            animationTrigger.Set();
        }

        private void OnGrounded() {
            if (Character.IsGrounded)
                _jumpCount = 0;
        }

    }

}