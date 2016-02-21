using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    public class Jump : BaseAnimationBehaviour<Character> {

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            animator.SetBool(CharacterAnimVars.JumpInput, false);
            if (!Target)
                return;
            Target.Jump();
        }

    }
}
