using UnityEngine;

namespace Genso.API {

    public sealed class CharacterAnimator : CharacterComponent {

        private Animator _animator;
        private CharacterPhysics _physics;

        [SerializeField]
        private AnimationBool _grounded = new AnimationBool("grounded");

        [SerializeField]
        private AnimationBool _helpless = new AnimationBool("helpless");

        [SerializeField]
        private AnimationFloat _verticalSpeed = new AnimationFloat("vertical speed");
        
        [SerializeField]
        private AnimationFloat _horizontalSpeed = new AnimationFloat("horizontal speed");

        [SerializeField]
        private AnimationTrigger _jump = new AnimationTrigger("jump");

        [SerializeField]
        private AnimationTrigger _airJump = new AnimationTrigger("air jump");

        protected override void Awake() {
            base.Awake();

            _animator = GetComponentInChildren<Animator>();
            _physics = GetComponent<CharacterPhysics>();

            _grounded.Animator = _animator;
            _helpless.Animator = _animator;
            _verticalSpeed.Animator = _animator;
            _horizontalSpeed.Animator = _animator;
            _jump.Animator = _animator;
            _airJump.Animator = _animator;

            _grounded.Set(Character.IsGrounded);
        }

        void Update() {
            _helpless.Set(Character.IsHelpless);
        }

        public override void OnJump() {
            base.OnJump();
            if(Character.IsGrounded)
                _jump.Set();
            else
                _airJump.Set();
        }

        public override void OnGrounded() {
            _grounded.Set(Character.IsGrounded);
        }

    }


}