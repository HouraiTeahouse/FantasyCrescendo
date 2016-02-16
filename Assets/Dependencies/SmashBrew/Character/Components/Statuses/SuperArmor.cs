using UnityEngine;
using System.Collections;

namespace HouraiTeahouse.SmashBrew {
    
    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    [RequireComponent(typeof(CharacterKnockback))]
    public sealed class SuperArmor : Status {

        //private CharacterKnockback _knockback;

        //TODO: Properly implement

        //protected override void Awake() {
        //    base.Awake();
        //    _knockback = GetComponent<CharacterKnockback>();
        //}

    }

}
