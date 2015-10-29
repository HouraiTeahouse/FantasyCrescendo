using UnityEngine;
using UnityEngine.Networking;
using System;

namespace Hourai.SmashBrew
{
    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    [RequireComponent(typeof(Rigidbody), typeof(Grounding))]
    public sealed class Jumping : RestrictableCharacterComponent
    {
        [SerializeField]
        private float[] _jumpPower = { 1.5f, 1.5f };
        
        [SerializeField]
        private GameObject airJumpFX;

        public event Action OnJump;

        private Rigidbody _rigidbody;
        private Grounding _ground;

        public int JumpCount { get; private set; }

        public int MaxJumpCount
        {
            get { return _jumpPower == null ? 0 : _jumpPower.Length; }
        }

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _ground = GetComponent<Grounding>();
            _ground.OnGrounded += OnGrounded;
        }

        void OnDestroy()
        {
            _ground.OnGrounded -= OnGrounded;
        }

        void OnGrounded()
        {
            if(_ground.IsGrounded)
                JumpCount = 0;
        }

        public void Jump()
        {
            if (JumpCount >= MaxJumpCount)//Restricted)
                return;

            // Apply upward force to jump
            _rigidbody.AddForce(Vector3.up * _jumpPower[JumpCount]);

            JumpCount++;

            // Trigger animation

            if (!_ground.IsGrounded && airJumpFX)
                Instantiate(airJumpFX, transform.position, Quaternion.Euler(90f, 0f, 0f));

            if (OnJump != null)
                OnJump();
        }
    }


}