using Hourai;
using UnityEngine;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    public class CharacterFalling : CharacterComponent {

        [SerializeField]
        private float _fastFallSpeed = 9f;

        [SerializeField]
        private float _maxFallSpeed = 5f;

        private bool _fastFall;

        public bool IsFastFalling {
            get { return Character != null && !Character.IsGrounded && _fastFall; }
        }

        public float FallSpeed {
            get { return IsFastFalling ? _fastFallSpeed : _maxFallSpeed; }
        }

        protected override void Start() {
            base.Start();
            Character.OnGrounded += OnGrounded;
        }

        private void OnGrounded() {
            if(Character.IsGrounded)
               _fastFall = false;
        }

        private void FixedUpdate() {
            if (Character == null)
                return;

            Vector3 velocity = Character.Velocity;
            
            if (!IsFastFalling && InputSource != null && InputSource.Crouch)
                _fastFall = true;

            if (IsFastFalling || velocity.y < -FallSpeed)
                velocity.y = -FallSpeed;

            Character.Velocity = velocity;
        }

    }

}
