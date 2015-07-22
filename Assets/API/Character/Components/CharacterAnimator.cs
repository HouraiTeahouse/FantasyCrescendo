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

        [SerializeField]
        private AnimationTrigger _attack = new AnimationTrigger("attack");

        [SerializeField]
        private AnimationFloat _horizontalInput = new AnimationFloat("horizontal input");

        [SerializeField]
        private AnimationFloat _verticalInput = new AnimationFloat("vertical input");

        protected override void Awake() {
            base.Awake();

            _animator = GetComponentInChildren<Animator>();
            _physics = GetComponent<CharacterPhysics>();

            // No point in continuing if an animator
            if (_animator == null) {
                enabled = false;
                Debug.LogWarning("Character Animator could not find a Animator component under " + name);
                return;
            }

            _grounded.Animator = _animator;
            _helpless.Animator = _animator;
            _verticalSpeed.Animator = _animator;
            _horizontalSpeed.Animator = _animator;
            _jump.Animator = _animator;
            _airJump.Animator = _animator;
            _attack.Animator = _animator;
            _horizontalInput.Animator = _animator;
            _verticalInput.Animator = _animator;

            _grounded.Set(Character.IsGrounded);

            if (Character == null)
                return;

            // Subscribe to Character events
            Character.OnGrounded += OnGrounded;
            Character.OnJump += OnJump;
            Character.OnAttack += OnAttack;
        }

        void OnDestroy() {
            if (Character == null)
                return;

            // Unsubscribe from Character events
            Character.OnGrounded -= OnGrounded;
            Character.OnJump -= OnJump;
            Character.OnAttack -= OnAttack;
        }

        protected override void OnUpdate() {
            _helpless.Set(Character.IsHelpless);

            if (_physics == null)
                return;

            Vector2 velocity = _physics.Velocity;
            _horizontalSpeed.Set(Mathf.Abs(velocity.x));
            _verticalSpeed.Set(velocity.y);

            if (InputSource == null)
                return;

            Vector2 movementInput = InputSource.Movement;
            _horizontalInput.Set(Mathf.Abs(movementInput.x));
            _verticalInput.Set(movementInput.y);
        }

        void OnAttack() {
            _attack.Set();
        }

        void OnJump() {
            if(Character.IsGrounded)
                _jump.Set();
            else
                _airJump.Set();
        }

       void OnGrounded() {
            _grounded.Set(Character.IsGrounded);
        }

    }


}