using System;
using UnityEngine;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    public class CharacterSpecial : RestrictableCharacterComponent {
        
        [SerializeField]
        private int _specialTrigger;

        public event Action OnSpecial;

        protected override void OnUpdate() {
            if (InputSource != null && InputSource.Special)
                Special();
        }

        public void Special() {
            if (Restricted)
                return;

            Animator.SetTrigger(_specialTrigger);

            if(OnSpecial != null)
                OnSpecial();
        }

    }

}
