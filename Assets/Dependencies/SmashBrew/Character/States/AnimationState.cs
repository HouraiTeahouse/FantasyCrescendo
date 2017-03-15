using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(MovementState))]
    [RequireComponent(typeof(PhysicsState))]
    public class AnimationState : NetworkBehaviour, ICharacterState {

        [SerializeField]
        Animator _animator;

        MovementState Movement { get; set; }
        PhysicsState Physics { get; set; }
        CharacterController CharacterController { get; set; }
        bool _jumpState = false;

        public Animator Animator {
            get { return _animator; }
        }

        void Awake() {
            if (_animator == null)
                _animator = this.SafeGetComponentInChildren<Animator>();
            Movement = this.SafeGetComponent<MovementState>();
            Physics = this.SafeGetComponent<PhysicsState>();
            CharacterController = this.SafeGetComponent<CharacterController>();
            if (Movement != null)
                Movement.OnJump += OnJump;
        }

        void OnDestroy() {
            if (Movement != null)
                Movement.OnJump -= OnJump;
        }

        float Sign(float x) {
            if (x > 0)
                return 1;
            if (x < 0)
                return -1;
            return 0;
        }

        void Update() {
            if (!hasAuthority || _animator == null || Movement == null)
                return;
            if (Mathf.Approximately(Time.deltaTime, 0))
                return;
            _animator.SetBool("grounded", Movement.IsGrounded);
            _animator.SetBool("ledge", Movement.CurrentLedge != null);
            _animator.SetBool("crouch", Movement.IsCrounching);
            _animator.SetBool("jump", _jumpState);
            _jumpState = false;
            var movement = Physics.Velocity.x;
            if (Mathf.Abs(movement) > Movement.FastWalkSpeed)
                movement = Sign(movement) * 2;
            else
                movement = Sign(movement);
            _animator.SetFloat("horizontal", movement);
        }

        void OnJump() { _jumpState = true; }

        void Reset() {
            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();
        }

        public void ResetState() {
            //TODO(james7132): implement
        }

    }

}

