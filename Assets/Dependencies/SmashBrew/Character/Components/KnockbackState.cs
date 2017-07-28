using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Characters {

    [RequireComponent(typeof(PhysicsState))]
    [RequireComponent(typeof(DamageState))]
    public class KnockbackState : CharacterComponent, IKnockbackable {

        public PhysicsState PhysicsState { get; private set;}
        public DamageState DamageState { get; private set;}

        public event Action<object, Vector2> OnHit;

        public ModifierGroup<object, Vector2> Modifiers { get; private set; }
        public ModifierList<object, float> KnockbackDamageModifiers { get; private set; }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            PhysicsState = this.SafeGetComponent<PhysicsState>();
            DamageState = this.SafeGetComponent<DamageState>();
            Modifiers = new ModifierGroup<object, Vector2>();
            KnockbackDamageModifiers = new ModifierList<object, float>();

            // Knockback Resistance
            KnockbackDamageModifiers.Add((src, damage) => {
                if (Character == null || CurrentState == null)
                    return damage;
                return Mathf.Max(0f, damage - CurrentState.Data.KnockbackResistance);
            });
        }

        public float GetKnockbackDamage() {
            return KnockbackDamageModifiers.Modifiy(this, DamageState.CurrentDamage);
        }

        public void Knockback(object source, Vector2 knockback) {
            if (CurrentState.Data.DamageType == ImmunityType.SuperArmor)
                return;
            var baseKnockback = knockback.magnitude;
            var unit = knockback.normalized;
            var hitbox = source as Hitbox;
            if (hitbox == null) {
                knockback = Modifiers.In.Modifiy(source, knockback);
                PhysicsState.Velocity = knockback;
                return;
            }
            var damage = GetKnockbackDamage();
            // TODO(james7132)
            // see: https://www.ssbwiki.com/Knockback#Formula
            var multiplier = (damage / 10f) + ((damage + hitbox.Damage) / 20f);
            multiplier *= (2f / (PhysicsState.Weight + 1)) * 1.4f;
            multiplier += 18f;
            multiplier *= hitbox.Scaling;
            multiplier += baseKnockback;
            PhysicsState.Velocity = Modifiers.In.Modifiy(source, multiplier * unit);
            OnHit.SafeInvoke(source, PhysicsState.Velocity);
        }

    }

}

