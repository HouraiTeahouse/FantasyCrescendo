using System;
using UnityEngine;

namespace Hourai.SmashBrew {
 
    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    public class CharacterAttack : RestrictableCharacterComponent {

        [SerializeField]
        private int _attackTrigger;

        public event Action OnAttack;

        protected override void OnUpdate() {
            base.OnUpdate();
            if(InputSource != null && InputSource.Attack)
                Attack();
        }

        public void Attack() {
            if (Restricted)
                return;

            Animator.SetTrigger(_attackTrigger);
            
            if(OnAttack != null)
                OnAttack();
        }

    }

}

