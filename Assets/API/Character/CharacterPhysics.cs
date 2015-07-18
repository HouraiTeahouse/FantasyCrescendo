using UnityEngine;
using System.Collections;

namespace Genso.API {

    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class CharacterPhysics : CharacterComponent {

        //Expected Frame Rate 60fps
        private const float dt = 1/60f;

        [SerializeField]
        private float _gravity = 9.86f;

        [SerializeField]
        private float _maxFallSpeed = 10f;

        [SerializeField]
        private float _fastFallSpeed = 15f;

        [SerializeField]
        private float _helplessFallSpeed = 10f;

        [SerializeField]
        private float[] _jumpHeight = new float[] { 3, 2 };

        private float FallSpeed {
            get {
                return -_maxFallSpeed;
            }
        }

        public Vector3 Velocity {
            get { return _rigidbody.velocity; }
            set { _rigidbody.velocity = value; }
        }

        public float Mass {
            get { return _rigidbody.mass; }
            set { _rigidbody.mass = value; }
        }
        
        private Rigidbody _rigidbody;
        private int _jumpCount = 0;
        private bool _grounded;

        protected override void Awake() {
            base.Awake();
            _rigidbody = GetComponent<Rigidbody>();
        }

        void FixedUpdate() {
            // Apply custom gravity
            _rigidbody.AddForce(0f, -_gravity, 0f);

            Vector3 velocity = Velocity;
            if (velocity.y < FallSpeed)
                velocity.y = FallSpeed;

            Velocity = velocity;
        }

        public override void OnGrounded() {
            if (Character.Grounded)
                _jumpCount = 0;
        }

        public override void OnJump() {
            Debug.Log(_jumpCount);
            // Cannot jump if already jumped the maximum number of times.
            if (_jumpCount >= _jumpHeight.Length)
                return;
            
            // Apply upward force to jump
            Velocity += Vector3.up * Mathf.Sqrt(2f * _gravity * _jumpHeight[_jumpCount]);

            _jumpCount++;
        }

    }

}