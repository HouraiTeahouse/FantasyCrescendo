using System;
using UnityEngine;

namespace Hourai.SmashBrew {

    public delegate float Modifier<T>(T source, float damage);

    [DisallowMultipleComponent]
    public class CharacterDamage : CharacterComponent {
        
        // The Damage Value used internally for calculation of various aspects of the game, like knockback
        protected internal float CurrentDamage { get; protected set; }

        public virtual void Damage(IDamager source, float damage) {
            CurrentDamage += damage;
        }

        public virtual void Heal(IHealer source, float damage) {
            CurrentDamage -= damage;
        }

    }
}