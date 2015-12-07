using UnityEngine;

namespace Hourai.SmashBrew {

    public class Jump : BaseAnimationBehaviour<Jumping> {

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            animator.SetBool(CharacterAnimVars.JumpInput, false);
            if (!Target)
                return;
            Target.Jump();
        }

    }
}
