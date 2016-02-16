using UnityEngine;
using System.Collections;

namespace HouraiTeahouse.SmashBrew {

    public class Move : BaseAnimationBehaviour<Movement> {

        [SerializeField]
        private float _baseSpeed = 3f;

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (!Target)
                return;
            
            Target.Move(_baseSpeed * stateInfo.speed);
        }
    }

}

