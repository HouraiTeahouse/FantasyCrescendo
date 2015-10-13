using System;
using UnityEngine;

namespace Hourai.SmashBrew {

    public delegate float Modifier<T>(T source, float damage);

    [DisallowMultipleComponent]
    public abstract class CharacterDamage : CharacterComponent {
        
        // The Damage Value used internally for calculation of various aspects of the game, like knockback
        public abstract float CurrentDamage { get; set; }

        public virtual void Damage(IDamager source, float damage) {
            CurrentDamage += damage;
        }

        public virtual void Heal(IHealer source, float damage) {
            CurrentDamage -= damage;
        }

    }

    public class CharacterPercentDamage : CharacterDamage {

        [SerializeField]
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