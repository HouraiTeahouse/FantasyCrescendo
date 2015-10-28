using UnityEngine;

namespace Hourai.SmashBrew {
    
    public class Invincibility : Status {

        private Damage _damage;

        protected override void Start() {
            base.Start();
            _damage = GetComponent<Damage>();
            if(_damage)
                _damage.DefensiveModifiers.Add(InvincibilityModifier, int.MaxValue);
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            if(_damage)
                _damage.DefensiveModifiers.Remove(InvincibilityModifier);
        }

        float InvincibilityModifier(object source, float damage) {
            return enabled ? damage : 0f;
        }

    }

}

