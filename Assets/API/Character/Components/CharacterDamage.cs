using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vexe.Runtime.Extensions;

namespace Crescendo.API {

    public delegate float DamageModifier(IDamageSource source, float damage);

    public delegate float HealingModifier(IHealingSource source, float damage);

    [DisallowMultipleComponent]
    public abstract class CharacterDamageable : CharacterComponent, IDamageable {
        
        public abstract float DamageValue { get; protected set; }
        
        // The Damage Value used internally for calculation of various aspects of the game, like knockback
        protected internal abstract float InternalDamage { get; protected set; }

        private List<DamageModifier> _damageModifiers;
        private List<HealingModifier> _healingModifiers;  

        public event Action<IHealingSource, float> OnHeal;
        public event Action<IDamageSource, float> OnDamage;

        public bool AddDamageModifier(DamageModifier modifier, int priority = 0) {
            if (modifier == null || _damageModifiers.Contains(modifier))
                return false;
            _damageModifiers.Add(modifier);
            return true;
        }

        public bool RemoveDamageModifier(DamageModifier modifier) {
            return _damageModifiers.Remove(modifier);
        }

        public bool AddHealingModifier(HealingModifier modifier, int priority = 0) {
            if (modifier == null || _healingModifiers.Contains(modifier))
                return false;
            _healingModifiers.Add(modifier);
            return true;
        }

        public bool RemoveHealingModifier(HealingModifier modifier) {
            return _healingModifiers.Remove(modifier);
        }

        public void Damage(IDamageSource source) {
            if (source == null)
                return;

            float damage = Mathf.Abs(source.BaseDamage);

            for (var i = 0; i < _damageModifiers.Count; i++)
                damage = _damageModifiers[i](source, damage);

            HandleDamage(source, damage);
            OnDamage.SafeInvoke(source, damage);
        }

        public void Heal(IHealingSource source) {
            if (source == null)
                return;

            float healing = Mathf.Abs(source.BaseHealing);

            for (var i = 0; i < _healingModifiers.Count; i++)
                healing = _healingModifiers[i](source, healing);

            HandleHealing(source, healing);
            OnHeal.SafeInvoke(source, healing);
        }

        protected virtual void HandleDamage(IDamageSource source, float damage) {
            InternalDamage += damage;
        }

        protected virtual void HandleHealing(IHealingSource source, float damage) {
            InternalDamage -= damage;
        }

    }

    public class CharacterDamage : CharacterDamageable {

        public override float DamageValue { get; protected set; }

        protected internal override float InternalDamage { get; protected set; }

    }

}