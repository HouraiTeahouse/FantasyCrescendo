using UnityEngine;
using System.Collections;

namespace Genso.API {


    public class CharacterAnimator : CharacterComponent {

        private Animator _animator;
        [SerializeField]
        private AnimationBool Grounded = new AnimationBool("grounded");

        [SerializeField]
        private AnimationFloat VerticalSpeed = new AnimationFloat("vertical speed");
        
        [SerializeField]
        private AnimationFloat HorizontalSpeed = new AnimationFloat("horizontal speed");

        [SerializeField]
        private AnimationTrigger JumpTrigger = new AnimationTrigger("jump");

        [SerializeField]
        private AnimationTrigger AirJumpTrigger = new AnimationTrigger("air jump");

        protected override void Awake() {
            base.Awake();
            _animator = GetComponentInChildren<Animator>();
            Grounded.Animator = _animator;
            VerticalSpeed.Animator = _animator;
            HorizontalSpeed.Animator = _animator;
            JumpTrigger.Animator = _animator;
            AirJumpTrigger.Animator = _animator;

            Grounded.Set(Character.Grounded);
        }

        public override void OnJump() {
            base.OnJump();
            if(Character.Grounded)
                JumpTrigger.Set();
            else
                AirJumpTrigger.Set();
        }

        public override void OnGrounded() {
            Grounded.Set(Character.Grounded);
        }

    }


}