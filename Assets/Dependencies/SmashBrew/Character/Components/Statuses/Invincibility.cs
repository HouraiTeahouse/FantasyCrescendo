using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    
    /// <summary>
    /// A Status effect that prevents players from taking damage while active.
    /// </summary>
    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    public sealed class Invincibility : Status {

        private Character _character;

        /// <summary>
        /// Unity callback. Called once before the object's first frame.
        /// </summary>
        protected override void Start() {
            base.Start();
            _character = GetComponent<Character>();
            _character.DamageModifiers.In.Add(InvincibilityModifier, int.MaxValue);
        }

        /// <summary>
        /// Unity callback. Called on object destruction.
        /// </summary>
        void OnDestroy() {
            if(_character)
                _character.DamageModifiers.In.Remove(InvincibilityModifier);
        }

        /// <summary>
        /// Invincibilty modifier. Negates all damage while active.
        /// </summary>
        float InvincibilityModifier(object source, float damage) {
            return enabled ? damage : 0f;
        }

    }

}

