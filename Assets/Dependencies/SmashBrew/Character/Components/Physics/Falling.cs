using UnityEngine;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    [RequireComponent(typeof(Rigidbody), typeof(Grounding))]
    public class Falling : CharacterComponent {

        [SerializeField]
        private float _fastFallSpeed = 9f;

        [SerializeField]
        private float _maxFallSpeed = 5f;

        public bool IsFastFalling { get; set; }

        public float FallSpeed {
            get { return IsFastFalling ? _fastFallSpeed : _maxFallSpeed; }
        }

        protected override void Start() {
            base.Start();
            CharacterEvents.Subscribe<GroundEvent>(OnGrounded);
        }

        void OnGrounded(GroundEvent eventArgs) {
            if(eventArgs.grounded)
                IsFastFalling = false;
        }

        void FixedUpdate() {
            Vector3 velocity = Rigidbody.velocity;
            
            //if (!IsFastFalling && InputSource != null && InputSource.Movement.y < 0)
            //    _fastFall = true;

            if (IsFastFalling || velocity.y < -FallSpeed)
                velocity.y = -FallSpeed;

            Rigidbody.velocity = velocity;
        }

    }

}
