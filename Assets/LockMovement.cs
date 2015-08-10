using UnityEngine;

namespace Crescendo.API {

    public class LockMovement : CharacterAnimationBehaviour {

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (Character == null)
                return;

            Character.CanMove = false;
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (Character == null)
                return;

            Character.CanMove = true;
        }

    }

}