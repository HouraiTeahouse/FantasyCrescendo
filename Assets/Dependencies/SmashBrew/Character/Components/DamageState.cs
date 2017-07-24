using System;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    [DisallowMultipleComponent]
    [AddComponentMenu("Smash Brew/Character/Damage State")]
    public class DamageState : CharacterNetworkComponent, IDamageable {

        [SyncVar, SerializeField]
        float _currentDamage;

        /// <summary> 
        /// The current internal damage value. Used for knockback calculations. 
        /// </summary>
        public float CurrentDamage {
            get { return _currentDamage; }
            set { _currentDamage = value; }
        }
        public float DefaultDamage { get; set; }

        public ModifierGroup<object, float> DamageModifiers { get; private set; }
        public ModifierGroup<object, float> HealingModifiers { get; private set; }

        public DamageType Type { get; set; }

        protected override void Awake() {
            base.Awake();
            DamageModifiers = new ModifierGroup<object, float>();
            HealingModifiers = new ModifierGroup<object, float>();
            Type = DamageType.Percent;
        }

        internal float ModifyDamage(float baseDamage, object source = null) {
            return DamageModifiers.Out.Modifiy(source, baseDamage);
        }

        public override void ResetState() { CurrentDamage = DefaultDamage; }

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

        public static implicit operator float(DamageState damage) {
            return damage == null ? 0f : damage.CurrentDamage;
        }
    }

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

}

