using UnityEngine;
using System.Collections;

namespace Hourai.SmashBrew {
    
    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    [RequireComponent(typeof(CharacterKnockback))]
    public sealed class SuperArmor : Status {

        private CharacterKnockback _knockback;
        
        void Awake() {
            _knockback = GetComponent<CharacterKnockback>();
        }

    }

}
