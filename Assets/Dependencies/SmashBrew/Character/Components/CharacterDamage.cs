using System;
using System.Collections.Generic;
using UnityEngine;
using Vexe.Runtime.Extensions;

namespace Hourai.SmashBrew {

    public delegate float Modifier<T>(T source, float damage);

    [DisallowMultipleComponent]
    public abstract class CharacterDamageable : CharacterComponent, IDamageable {
        
        // The Damage Value used internally for calculation of various aspects of the game, like knockback
        protected internal abstract float InternalDamage { get; protected set; }

        private PriorityList<Modifier<IDamager>> _damageModifiers; 
        private PriorityList<Modifier<IHealer>> _healingModifiers;  

        public event Action<IHealer, float> OnHeal;
        public event Action<IDamager, float> OnDamage;

        void Awake() {
            _damageModifiers = new PriorityList<Modifier<IDamager>>();
            _healingModifiers = new PriorityList<Modifier<IHealer>>();
        }

        public bool AddDamageModifier(Modifier<IDamager> modifier, int priority = 0)
        {
            if (modifier == null || _damageModifiers.Contains(modifier))
                return false;
            _damageModifiers.Add(modifier);
            return true;
        }

        public bool RemoveDamageModifier(Modifier<IDamager> modifier)
        {
            return _damageModifiers.Remove(modifier);
        }

        public bool AddHealingModifier(Modifier<IHealer> modifier, int priority = 0)
        {
            if (modifier == null || _healingModifiers.Contains(modifier))
                return false;
            _healingModifiers.Add(modifier);
            return true;
        }

        public bool RemoveHealingModifier(Modifier<IHealer> modifier)
        {
            return _healingModifiers.Remove(modifier);
        }

        public void Damage(IDamager source) {
            if (source == null)
                return;

            float damage = Mathf.Abs(source.BaseDamage);

            foreach(var modifier in _damageModifiers)
                damage = modifier(source, damage);

            HandleDamage(source, damage);
            OnDamage.SafeInvoke(source, damage);
        }

        public void Heal(IHealer source) {
            if (source == null)
                return;

            float healing = Mathf.Abs(source.BaseHealing);

            foreach (var modifier in _healingModifiers)
                healing = modifier(source, healing);

            HandleHealing(source, healing);
            OnHeal.SafeInvoke(source, healing);
        }

        protected virtual void HandleDamage(IDamager source, float damage) {
            InternalDamage += damage;
        }

        protected virtual void HandleHealing(IHealer source, float damage) {
            InternalDamage -= damage;
        }

    }

    public class CharacterDamage : CharacterDamageable {

        protected internal override float InternalDamage { get; protected set; }

    }

}