using Hourai.Events;
using UnityEngine;

namespace Hourai.SmashBrew
{
    public class JumpEvent : IEvent {

        public bool ground;
        public int remainingJumps;

    }

    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class Jumping : RestrictableCharacterComponent
    {
        [SerializeField]
        private float[] _jumpPower = { 1.5f, 1.5f };
        
        private bool _grounded;

        public int JumpCount { get; private set; }

        public int MaxJumpCount
        {
            get { return _jumpPower == null ? 0 : _jumpPower.Length; }
        }

        protected override void Start() {
            base.Start();
            CharacterEvents.Subscribe<GroundEvent>(OnGrounded);
        }

        void OnDestroy()
        {
            CharacterEvents.Unsubscribe<GroundEvent>(OnGrounded);
        }

        void OnGrounded(GroundEvent eventArgs) {
            _grounded = eventArgs.grounded;
            if(_grounded)
                JumpCount = 0;
        }

        public void Jump()
        {
            if (JumpCount >= MaxJumpCount)//Restricted)
                return;

            // Apply upward force to jump
            Rigidbody.AddForce(Vector3.up * _jumpPower[JumpCount]);

            JumpCount++;

            CharacterEvents.Publish(new JumpEvent { ground = _grounded, remainingJumps = MaxJumpCount - JumpCount });
        }
    }


}