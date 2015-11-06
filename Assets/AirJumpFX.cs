using UnityEngine;
using System.Collections;

namespace Hourai.SmashBrew {

    public class AirJumpFX : BaseAnimationBehaviour<Transform> {

        [SerializeField]
        private GameObject Effect;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (Effect && Target)
                Instantiate(Effect, Target.position, Effect.transform.rotation);
        }

    }


}
