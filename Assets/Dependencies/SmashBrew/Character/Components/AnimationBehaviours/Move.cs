using UnityEngine;
using System.Collections;

namespace HouraiTeahouse.SmashBrew {

    public class Move : BaseAnimationBehaviour<Character> {

        [SerializeField]
        private float _baseSpeed = 3f;

        [SerializeField]
        private bool _ignoreStateSpeed;

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (!Target)
                return;
            
            if(_ignoreStateSpeed)
                Target.Move(_baseSpeed);
            else
                Target.Move(_baseSpeed * stateInfo.speed);
        }
    }

}

