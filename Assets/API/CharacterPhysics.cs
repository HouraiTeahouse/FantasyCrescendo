using UnityEngine;
using System.Collections;

namespace Genso.API {


    [RequireComponent(typeof(CharacterController))]
    public sealed class CharacterPhysics : MonoBehaviour {

        //Expected Frame Rate 60fps
        private const float dt = 1/60f;

        [SerializeField]
        private float _mass = 1f;

        [SerializeField]
        private float _gravity = 9.86f;

        [SerializeField]
        private float _maxFallSpeed = 10f;

        [SerializeField]
        private float _fastFallSpeed = 15f;

        [SerializeField]
        private float[] _jumpPower = new float[] {10, 5};
        
        private CharacterController _controller;
        private int jumpCount = 0;
        private Vector3 _acceleration;
        private Vector3 _velocity;
        private bool _grounded;

        private void Awake() {
            _controller = GetComponent<CharacterController>();
        }

        void Update() {
            ApplyGravity();
            _velocity += _acceleration * dt;
            Debug.Log(_velocity);
            _controller.Move(_velocity * dt);
        }

        public void Jump() {
            // Cannot jump if already jumped the maximum number of times.
            if (jumpCount >= _jumpPower.Length)
                return;
            
            // Apply upward force to jump
            ApplyForce(0f, _jumpPower[jumpCount], 0f);
            jumpCount++;
        }

        public void ApplyForce(Vector3 force) {
            _acceleration += force/_mass;
        }

        public void ApplyForce(float x, float y, float z) {
            ApplyForce(new Vector3(x, y, z));
        }

        void ApplyGravity() {
            _velocity.y -= _gravity * dt;
            if (_velocity.y <= -_maxFallSpeed)
                _velocity.y = -_maxFallSpeed;
        }

    }

}