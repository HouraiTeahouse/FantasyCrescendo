using Hourai;
using UnityEngine;

namespace Genso.API {

    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    public class CharacterFalling : CharacterComponent {

        [SerializeField]
        private float _fastFallSpeed = 9f;

        [SerializeField]
        private float _maxFallSpeed = 5f;

        [SerializeField]
        private bool _fastFallInputted = false;

        public bool IsFastFalling {
            get { return Character != null && InputSource != null && !Character.IsGrounded && _fastFallInputted;}
        }

        public float FallSpeed {
            get { return IsFastFalling ? _fastFallSpeed : _maxFallSpeed; }
        }

        private void FixedUpdate() {
            Vector3 velocity = Character.Velocity;

            
            if (!IsFastFalling && InputSource != null && InputSource.Crouch){
                _fastFallInputted = true;    
            }
            
            if (Character.IsGrounded){
                _fastFallInputted = false;
            }

            if (IsFastFalling || velocity.y < -FallSpeed)
                velocity.y = -FallSpeed;

            Character.Velocity = velocity;
        }

    }

}
