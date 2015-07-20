using System;
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
        private float _walkSpeed = 3f;

        [SerializeField]
        private float _dashSpeed = 5f;

        [SerializeField]
        private float _airSpeed = 3f;

        [SerializeField]
        private float _maxFallSpeed = 10f;

        [SerializeField]
        private float _fastFallSpeed = 15f;

        [SerializeField]
        private float _helplessFallSpeed = 10f;

        [SerializeField]
        private float[] _jumpHeight = new float[] { 1.5f, 1.5f };

        private float FallSpeed {
            get {
                if (Character.IsHelpless)
                    return _helplessFallSpeed;
                if(Character.IsFastFalling)
                    return _fastFallSpeed;
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

            if (Character == null)
                return;

            // Subscribe to Character events
            Character.OnMove += OnMove;
            Character.OnGrounded += OnGrounded;
            Character.OnJump += OnJump;
        }

        void OnDestroy() {
            if (Character == null)
                return;

            // Unsubscribe to Character events
            Character.OnMove -= OnMove;
            Character.OnGrounded -= OnGrounded;
            Character.OnJump += OnJump;
        }

        private void FixedUpdate() {
            // Apply custom gravity
            _rigidbody.AddForce(0f, -_gravity, 0f);

            Vector3 velocity = Velocity;
            if (velocity.y < FallSpeed)
                velocity.y = FallSpeed;

            Velocity = velocity;
        }

        void OnMove(Vector2 direction) {
            if (Math.Abs(direction.x) < float.Epsilon)
                return;

            Vector3 vel = Velocity;

            if (Character.IsGrounded) {
                if (Character.IsDashing)
                    vel.x = _dashSpeed;
                else
                    vel.x = _walkSpeed;
            } else {
                vel.x = _airSpeed;
            }
            vel.x = Util.MatchSign(vel.x, direction.x);

            Velocity = vel;

        }

        void OnGrounded() {
            if (Character.IsGrounded)
                _jumpCount = 0;
        }

        void OnJump() {
            // Cannot jump if already jumped the maximum number of times.
            if (_jumpCount >= _jumpHeight.Length - 1)
                return;
            
            // Apply upward force to jump
            Velocity += Vector3.up * Mathf.Sqrt(2f * _gravity * _jumpHeight[_jumpCount]);

            _jumpCount++;
        }

    }

}