using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    /// <summary> A AnimationBehaviour that spawns an effect at the base of the object that is being animated </summary>
    public class AirJumpFX : BaseAnimationBehaviour<Transform> {

        [SerializeField]
        GameObject Effect;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (Effect && Target)
                Instantiate(Effect, Target.position, Effect.transform.rotation);
        }

    }

}