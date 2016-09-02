using System;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    public class DamageType {

        public static readonly DamageType Percent = new DamageType {
            _change = (currentDamage, delta) => currentDamage + delta,
            Suffix = "%",
            Range = new Range(0, 999)
        };

        public static readonly DamageType Stamina = new DamageType {
            _change = (currentDamage, delta) => currentDamage + delta,
            Suffix = "HP",
            Range = new Range(0, 999)
        };

        Func<float, float, float> _change;

        DamageType() { }
        public string Suffix { get; private set; }
        public Range Range { get; private set; }

        public float Damage(float currentDamage, float delta) {
            return Range.Clamp(_change(currentDamage, Mathf.Abs(delta)));
        }

        public float Heal(float currentDamage, float delta) {
            return Range.Clamp(_change(currentDamage, -Mathf.Abs(delta)));
        }

    }

    /// <summary> A MonoBehaviour that handles all of the damage dealt and recieved by a character. </summary>
    public sealed class PlayerDamage : HouraiBehaviour, IResettable {

        /// <summary> The current internal damage value. Used for knockback calculations. </summary>
        public float CurrentDamage { get; set; }

        public float DefaultDamage { get; set; }

        public DamageType Type { get; set; }
        public ModifierGroup<object> DamageModifiers { get; private set; }
        public ModifierGroup<object> HealingModifiers { get; private set; }

        public void OnReset() { CurrentDamage = DefaultDamage; }

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

        public void Damage(float damage) { Damage(null, damage); }

        public void Damage(object source, float damage) {
            damage = DamageModifiers.In.Modifiy(source, Mathf.Abs(damage));
            CurrentDamage = Type.Damage(CurrentDamage, damage);
        }

        public void Heal(float healing) { Heal(null, healing); }

        public void Heal(object source, float healing) {
            healing = HealingModifiers.In.Modifiy(source, Mathf.Abs(healing));
            CurrentDamage = Type.Heal(CurrentDamage, healing);
        }

    }

}