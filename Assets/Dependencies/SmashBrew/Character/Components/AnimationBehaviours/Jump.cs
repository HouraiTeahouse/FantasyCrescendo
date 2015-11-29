using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Director;

namespace Hourai.SmashBrew {

    public class Jump : BaseAnimationBehaviour<Jumping> {

        private Rigidbody _rigidbody;

        public override void Initialize(GameObject gameObject) {
            base.Initialize(gameObject);
            _rigidbody = gameObject.GetComponent<Rigidbody>();
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            animator.SetBool(CharacterAnimVars.JumpInput, false);
            if (!Target)
                return;
            Target.Jump();
        }
    }
}
