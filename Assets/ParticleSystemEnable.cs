using UnityEngine;
using System.Collections;

namespace HouraiTeahouse.SmashBrew {

    public class ParticleSystemEnable : BaseAnimationBehaviour<Character> {

        [SerializeField]
        private int particleIndex;

        [SerializeField, Range(0f, 1f)]
        private float toggleTime;

        private bool toggled;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            toggled = false;
            if(Target)
                Target.SetParticleVisibilty(particleIndex, true);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (Target && stateInfo.normalizedTime > toggleTime && !toggled) {
                Debug.Log(stateInfo.normalizedTime);
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
