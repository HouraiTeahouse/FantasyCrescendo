using UnityEngine;
using System.Collections;
namespace HouraiTeahouse.SmashBrew {

    public class WeaponVisibilty : BaseAnimationBehaviour<Character> {

        [SerializeField]
        private int weapon;

        [SerializeField]
        private bool state;

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
