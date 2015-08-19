using System;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    public class CharacterAttack : RestrictableCharacterComponent {

        [Serialize, Show, AnimVar(Filter = ParameterType.Trigger, AutoMatch = "Trigger")]
        private int _attackTrigger;

        public event Action OnAttack;

        void Update() {
            if(InputSource != null && InputSource.Attack)
                Attack();
        }

        public void Attack() {
            if (Restricted)
                return;

            Animator.SetTrigger(_attackTrigger);

            OnAttack.SafeInvoke();
        }

    }

}

