using UnityEngine;

namespace Crescendo.API {
    
    public class CharacterJump : CharacterComponent {
        
        private CharacterGravity _gravity;
        private int _jumpCount = 0;

        private const float defaultGravity = 9.86f;
        
        [SerializeField]
        private float[] _jumpHeights = new float[] { 1.5f, 1.5f };

        protected override void Start() {
            base.Start();
            _gravity = GetComponentInChildren<CharacterGravity>();
            Character.OnJump += OnJump;
            Character.OnGrounded += OnGrounded;
        }

        void OnJump() {
            // Cannot jump if already jumped the maximum number of times.
            if (_jumpCount >= _jumpHeights.Length)
                return;

            var g = _gravity == null ? _gravity.Gravity : defaultGravity;

            // Apply upward force to jump
            Vector3 temp = Character.Velocity;
            temp.y = Mathf.Sqrt(2 * g * _jumpHeights[_jumpCount]);
            Character.Velocity = temp;

            _jumpCount++;
        }

        void OnGrounded() {
            if (Character.IsGrounded)
                _jumpCount = 0;
        }

    }


}
