using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    public class ParticleSystemEnable : BaseAnimationBehaviour<Character> {

        [SerializeField]
        int particleIndex;

        bool toggled;

        [SerializeField]
        [Range(0f, 1f)]
        float toggleTime;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            toggled = false;
            if (Target)
                Target.SetParticleVisibilty(particleIndex, true);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (Target && stateInfo.normalizedTime > toggleTime && !toggled) {
                Target.SetParticleVisibilty(particleIndex, true);
                toggled = true;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (Target)
                Target.SetParticleVisibilty(particleIndex, false);
        }

    }

}