using UnityEngine;

namespace Hourai {

    public class LockMovement : CharacterAnimationBehaviour {

        protected override Character Character {
            get { return base.Character; }
            set {
                base.Character = value;
                if (value != null)
                    _movement = value.GetComponentInChildren<CharacterMovement>();
            }
        }

        private CharacterMovement _movement;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (Character == null)
                return;
            if (_movement == null) {
                _movement = Character.GetComponentInChildren<CharacterMovement>();
                if (_movement == null)
                    return;
            }

            _movement.Restricted = true;
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (Character == null)
                return;
            if (_movement == null) {
                _movement = Character.GetComponentInChildren<CharacterMovement>();
                if (_movement == null)
                    return;
            }

            _movement.Restricted = false;
        }

    }

}