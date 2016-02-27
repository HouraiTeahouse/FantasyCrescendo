using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    /// <summary>
    /// A AnimationBehaviour that cancels all vertical momentum on entry into a state
    /// </summary>
    public class DisableGravity : BaseAnimationBehaviour<Rigidbody> {

        private RigidbodyConstraints _oldConstraints;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (!Target)
                return;
            _oldConstraints = Target.constraints;
            Target.constraints = _oldConstraints | RigidbodyConstraints.FreezePositionY;
            Vector3 velocity = Target.velocity;
            velocity.y = 0f;
            Target.velocity = velocity;

        }

    }


}
