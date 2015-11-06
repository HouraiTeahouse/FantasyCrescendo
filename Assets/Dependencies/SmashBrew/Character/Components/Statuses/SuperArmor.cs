using UnityEngine;
using System.Collections;

namespace Hourai.SmashBrew {
    
    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    [RequireComponent(typeof(CharacterKnockback))]
    public class SuperArmor : Status {

        private CharacterKnockback _knockback;

        protected override void Awake() {
            _knockback = GetComponent<CharacterKnockback>();
            base.Awake();
        }

    }

}
