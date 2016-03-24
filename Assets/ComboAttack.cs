using UnityEngine;
using System.Collections;

namespace HouraiTeahouse.SmashBrew {

    public class ComboAttack : StateMachineBehaviour {

        [SerializeField]
        private bool resetOnExit;

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (animator.GetBool(CharacterAnim.AttackInput))
                animator.SetInteger(CharacterAnim.Combo, animator.GetInteger(CharacterAnim.Combo) + 1);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if(resetOnExit)
                animator.SetInteger(CharacterAnim.Combo, 0);
            else
                animator.SetInteger(CharacterAnim.Combo, animator.GetInteger(CharacterAnim.Combo) - 1);
        }

    }
}
