using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    /// <summary>
    /// A Status effect that prevents players from taking damage while active.
    /// </summary>
    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    public sealed class Invincibility : Status {
        private PlayerDamage _damage;
        private Hitbox[] _hitboxes;

        /// <summary>
        /// Unity callback. Called once before the object's first frame.
        /// </summary>
        protected override void Start() {
            base.Start();
            _damage = GetComponent<PlayerDamage>();
            _damage.DamageModifiers.In.Add(InvincibilityModifier, int.MaxValue);
            _hitboxes = GetComponentsInChildren<Hitbox>();
        }

        /// <summary>
        /// Unity callback. Called on object destruction.
        /// </summary>
        void OnDestroy() {
            if (_damage)
                _damage.DamageModifiers.In.Remove(InvincibilityModifier);
        }

        /// <summary>
        /// Unity callback. Called when component is enabled.
        /// </summary>
        void OnEnable() {
            if (_hitboxes == null)
                _hitboxes = GetComponentsInChildren<Hitbox>();
            foreach (Hitbox hitbox in _hitboxes)
                if (hitbox.CurrentType == Hitbox.Type.Damageable)
                    hitbox.CurrentType = Hitbox.Type.Invincible;
        }

        /// <summary>
        /// Unity callback. Called when component is disabled.
        /// </summary>
        void OnDisable() {
            if (_hitboxes == null)
                _hitboxes = GetComponentsInChildren<Hitbox>();
            foreach (var hitbox in _hitboxes)
                if (hitbox.CurrentType == Hitbox.Type.Invincible)
                    hitbox.CurrentType = Hitbox.Type.Damageable;
        }

        /// <summary>
        /// Invincibilty modifier. Negates all damage while active.
        /// </summary>
        float InvincibilityModifier(object source, float damage) {
            return enabled ? damage : 0f;
        }
    }
}
