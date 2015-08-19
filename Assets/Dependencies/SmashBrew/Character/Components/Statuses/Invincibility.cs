using UnityEngine;
using System.Collections;

namespace Hourai.SmashBrew {
    
    public class Invincibility : Status {

        private CharacterDamageable damageable;

        protected override void OnStatusStart() {
            damageable = Character.GetComponent<CharacterDamageable>();
            if(damageable == null)
                EndStatus();
            else {
                damageable.AddDamageModifier(InvincibilityModifier, int.MaxValue);
            }
        }

        protected override void OnStatusUpdate() {
        }

        protected override void OnStatusEnd() {
            if (damageable != null)
                damageable.RemoveDamageModifier(InvincibilityModifier);
        }

        static float InvincibilityModifier(IDamageSource source, float damage) {
            return 0f;
        }

    }

}

