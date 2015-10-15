using System;
using UnityEngine;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    public abstract class CharacterDamage : CharacterComponent {
        
        // The Damage Value used internally for calculation of various aspects of the game, like knockback
        public abstract float CurrentDamage { get; set; }

        protected const float MinDamage = 0f;
        protected const float MaxDamage = 999.9999f;

        public virtual void Damage(IDamager source, float damage) {
            CurrentDamage += damage;
            if (CurrentDamage > MaxDamage)
                CurrentDamage = MaxDamage;
        }

        public virtual void Heal(IHealer source, float damage) {
            CurrentDamage -= damage;
            if (CurrentDamage < MinDamage)
                CurrentDamage = MinDamage;
        }

    }

    public class CharacterPercentDamage : CharacterDamage {

        [SerializeField, Range(MinDamage, MaxDamage)]
        private float _damage = 0f;

        public override float CurrentDamage {
            get {
                return _damage;
            }
            set {
                _damage = value;
            }
        }

    }

}