using UnityEngine;
using System.Collections;

namespace Genso.API {

    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class CharacterPhysics : CharacterComponent {

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
        
        private Rigidbody _rigidbody;
        private int jumpCount = 0;
        private bool _grounded;

        protected override void Awake() {
            base.Awake();
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Jump() {
            // Cannot jump if already jumped the maximum number of times.
            if (jumpCount >= _jumpPower.Length)
                return;
            
            // Apply upward force to jump
            _rigidbody.AddForce(0f, _jumpPower[jumpCount], 0f);
            jumpCount++;
        }

    }

}