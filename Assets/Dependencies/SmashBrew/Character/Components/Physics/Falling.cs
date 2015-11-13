using Hourai;
using UnityEngine;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    [RequireComponent(typeof(Rigidbody), typeof(Grounding))]
    public class Falling : HouraiBehaviour {

        [SerializeField]
        private float _fastFallSpeed = 9f;

        [SerializeField]
        private float _maxFallSpeed = 5f;

        private Grounding _grounded;
        private bool _fastFall;

        public bool IsFastFalling {
            get { return _grounded.IsGrounded && _fastFall; }
        }

        public float FallSpeed {
            get { return IsFastFalling ? _fastFallSpeed : _maxFallSpeed; }
        }

        void Awake() {
            _grounded = GetComponent<Grounding>();
            _grounded.OnGrounded += OnGrounded;
        }

        void OnGrounded() {
            if(_grounded.IsGrounded)
               _fastFall = false;
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
