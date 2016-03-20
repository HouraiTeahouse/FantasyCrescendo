using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    /// <summary>
    /// An AnimationBehaviour that causes Characters to jump on entry into a state.
    /// </summary>
    public class Jump : BaseAnimationBehaviour<Character> {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            animator.SetBool(CharacterAnim.JumpInput, false);
            if (!Target)
                return;
            Target.Jump();
        }
    }
}
