using UnityEngine;
using System.Collections;

namespace Hourai.SmashBrew
{
    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class Jumping : RestrictableCharacterComponent
    {
        [SerializeField]
        private float[] _jumpPower = { 1.5f, 1.5f };
        
        [SerializeField]
        private GameObject airJumpFX;

        private Rigidbody _rigidbody;

        public int JumpCount { get; private set; }

        public int MaxJumpCount
        {
            get { return _jumpPower == null ? 0 : _jumpPower.Length; }
        }

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        protected override void Start()
        {
            base.Start();
            if (Character)
                Character.OnGrounded += OnGrounded;
        }

        void OnDestroy()
        {
            if (Character)
                Character.OnGrounded -= OnGrounded;
        }

        void OnGrounded()
        {
            if (Character && Character.IsGrounded)
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

            if (!Character.IsGrounded && airJumpFX)
                Instantiate(airJumpFX, transform.position, Quaternion.Euler(90f, 0f, 0f));

            if (OnJump != null)
                OnJump();
        }
    }


}