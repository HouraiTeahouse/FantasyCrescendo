using System;
using HouraiTeahouse.SmashBrew.Util;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    public class DamageType {
        public string Suffix { get; private set; }
        public float MinDamage { get; private set; }
        public float MaxDamage { get; private set; }
        private Func<float, float, float> Change;

        private DamageType() {
        }

        public static readonly DamageType Percent = new DamageType {
            Change = (currentDamage, delta) => currentDamage + delta,
            Suffix = "%",
            MaxDamage = 999f,
            MinDamage = 0f
        };

        public static readonly DamageType Stamina = new DamageType {
            Change = (currentDamage, delta) => currentDamage + delta,
            Suffix = "HP",
            MaxDamage = 999f,
            MinDamage = 0f
        };

        public float Damage(float currentDamage, float delta) {
            return Mathf.Clamp(Change(currentDamage, Mathf.Abs(delta)), MinDamage, MaxDamage);
        }

        public float Heal(float currentDamage, float delta) {
            return Mathf.Clamp(Change(currentDamage, -Mathf.Abs(delta)), MinDamage, MaxDamage);
        }
    }

    /// <summary>
    /// A MonoBehaviour that handles all of the damage dealt and recieved by a character.
    /// </summary>
    public sealed class PlayerDamage : HouraiBehaviour, IResettable {

        /// <summary>
        /// The current internal damage value. Used for knockback calculations.
        /// </summary>
        public float CurrentDamage { get; set; }

        public float DefaultDamage { get; set; }

        public DamageType Type { get; set; }
        public ModifierGroup<object> DamageModifiers { get; private set; }
        public ModifierGroup<object> HealingModifiers { get; private set; }

        public static implicit operator float(PlayerDamage damage) {
            return damage == null ? 0f : damage.CurrentDamage;
        }

        protected override void Awake() {
            base.Awake();
            DamageModifiers = new ModifierGroup<object>();
            HealingModifiers = new ModifierGroup<object>();
        }

        internal float ModifyDamage(float baseDamage, object source = null) {
            return DamageModifiers.Out.Modifiy(source, baseDamage);
        }

        public void Damage(float damage) {
            Damage(null, damage);
        }

        public void Damage(object source, float damage) {
            damage = DamageModifiers.In.Modifiy(source, Mathf.Abs(damage));
            CurrentDamage = Type.Damage(CurrentDamage, damage);
        }

        public void Heal(float healing) {
            Heal(null, healing);
        }

        public void Heal(object source, float healing) {
            healing = HealingModifiers.In.Modifiy(source, Mathf.Abs(healing));
            CurrentDamage = Type.Heal(CurrentDamage, healing);
        }

        public void OnReset() {
            CurrentDamage = DefaultDamage;
        }
    }
}
