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
    [RequireComponent(typeof(Rigidbody), typeof(Grounding))]
    public sealed class Jumping : RestrictableCharacterComponent
    {
        [SerializeField]
        private float[] _jumpPower = { 1.5f, 1.5f };
        
        [SerializeField]
        private GameObject airJumpFX;

        private Grounding _ground;

        public int JumpCount { get; private set; }

        public int MaxJumpCount
        {
            get { return _jumpPower == null ? 0 : _jumpPower.Length; }
        }

        protected override void Awake() {
            base.Awake();
            _ground = GetComponent<Grounding>();
            CharacterEvents.Subscribe<GroundEvent>(OnGrounded);
        }

        void OnDestroy()
        {
            CharacterEvents.Unsubscribe<GroundEvent>(OnGrounded);
        }

        void OnGrounded(GroundEvent eventArgs)
        {
            if(eventArgs.grounded)
                JumpCount = 0;
        }

        public void Jump()
        {
            if (JumpCount >= MaxJumpCount)//Restricted)
                return;

            // Apply upward force to jump
            Rigidbody.AddForce(Vector3.up * _jumpPower[JumpCount]);

            JumpCount++;

            // Trigger animation

            if (!_ground.IsGrounded && airJumpFX)
                Instantiate(airJumpFX, transform.position, Quaternion.Euler(90f, 0f, 0f));

            CharacterEvents.Publish(new JumpEvent { ground = _ground.IsGrounded, remainingJumps = MaxJumpCount - JumpCount });
        }
    }


}