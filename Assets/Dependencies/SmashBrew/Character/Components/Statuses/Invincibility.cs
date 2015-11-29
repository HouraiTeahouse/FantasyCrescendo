using UnityEngine;

namespace Hourai.SmashBrew {
    
    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    public class Invincibility : Status {

        private Damage _damage;

        protected override void Start() {
            _damage = GetComponent<Damage>();
            if(_damage)
                _damage.DefensiveModifiers.Add(InvincibilityModifier, int.MaxValue);
        }

        private void OnDestroy() {
            if(_damage)
                _damage.DefensiveModifiers.Remove(InvincibilityModifier);
        }

        float InvincibilityModifier(object source, float damage) {
            return enabled ? damage : 0f;
        }

    }

}

