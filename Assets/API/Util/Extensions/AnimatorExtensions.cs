using UnityEngine;
using System.Collections;

namespace Crescendo.API {

    public static class AnimatorExtensions
    {

        public static bool IsInState(this Animator animator, int layerIndex, string stateName) {
            return animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
        }

        public static bool IsInState(this Animator animator, int layerIndex, int stateHash) {
            return animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == stateHash;
        }

        public static bool IsInState(this Animator animator, string stateName) {
            return IsInState(animator, 0, stateName);
        }

        public static bool IsInState(this Animator animator, int stateHash) {
            return IsInState(animator, 0, stateHash);
        }

    }

}
