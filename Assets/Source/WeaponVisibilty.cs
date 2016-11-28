using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    public class WeaponVisibilty : BaseAnimationBehaviour<Character> {

        [SerializeField]
        bool state;

        [SerializeField]
        int weapon;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (Target)
                Target.SetWeaponVisibilty(weapon, state);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (Target)
                Target.SetWeaponVisibilty(weapon, !state);
        }

    }

}