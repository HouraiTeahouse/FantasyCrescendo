using System;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    public class CharacterSpecial : RestrictableCharacterComponent {
        
        [Serialize, Show, AnimVar(Filter = ParameterType.Trigger, AutoMatch = "Trigger")]
        private int _specialTrigger;

        public event Action OnSpecial;

        void Update() {
            if (InputSource != null && InputSource.Special)
                Special();
        }

        public void Special() {
            if (Restricted)
                return;

            Animator.SetTrigger(_specialTrigger);

            OnSpecial.SafeInvoke();
        }

    }

}
