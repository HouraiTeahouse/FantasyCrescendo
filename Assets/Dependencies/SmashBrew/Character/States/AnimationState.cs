using System.Collections;
using System.Collections.Generic;
using HouraiTeahouse.SmashBrew;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    [RequireComponent(typeof(MovementState))]
    public class AnimationState : NetworkBehaviour, ICharacterState {

        [SerializeField]
        Animator _animator;

        MovementState Movement { get; set; }
        bool _jumpState = false;

        public Animator Animator {
            get { return _animator; }
        }

        void Awake() {
            if (_animator == null)
                _animator = this.SafeGetComponentInChildren<Animator>();
            Movement = this.SafeGetComponent<MovementState>();
            if (Movement != null)
                Movement.OnJump += OnJump;
        }

        void OnDestroy() {
            if (Movement != null)
                Movement.OnJump -= OnJump;
        }

        void Update() {
            if (!hasAuthority || _animator == null || Movement == null)
                return;
            //_animator.SetBool("grounded", .IsGrounded);
            _animator.SetBool("ledge", Movement.CurrentLedge != null);
            _animator.SetBool("jump", _jumpState);
            _jumpState = false;
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

