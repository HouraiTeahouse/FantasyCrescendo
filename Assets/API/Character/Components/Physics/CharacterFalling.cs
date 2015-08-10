using Crescendo.API;
using UnityEngine;

namespace Genso.API {

    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    public class CharacterFalling : CharacterComponent {

        [SerializeField]
        private float _fastFallSpeed = 9f;

        [SerializeField]
        private float _maxFallSpeed = 5f;

        private float FallSpeed {
            get { return Character.IsFastFalling ? _fastFallSpeed : _maxFallSpeed; }
        }

        private void FixedUpdate() {
            Vector3 velocity = Character.Velocity;

            if (Character.IsFastFalling || velocity.y < -FallSpeed)
                velocity.y = -FallSpeed;

            Character.Velocity = velocity;
        }

    }

}