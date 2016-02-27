using System;
using HouraiTeahouse.SmashBrew.Util;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    public class DamageType {

        public string Suffix { get; private set; }
        public float MinDamage { get; private set; }
        public float MaxDamage { get; private set; }

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

        private Func<float, float, float> Change;

        public float Damage(float currentDamage, float delta) {
            float newDamage = Change(currentDamage, Mathf.Abs(delta));
            return Mathf.Clamp(newDamage, MinDamage, MaxDamage);
        }

        public float Heal(float currentDamage, float delta) {
            float newDamage = Change(currentDamage, -Mathf.Abs(delta));
            return Mathf.Clamp(newDamage, MinDamage, MaxDamage);
        }

    }

    /// <summary>
    /// A MonoBehaviour that handles all of the damage dealt and recieved by a character.
    /// </summary>
    public partial class Character {

        /// <summary>
        /// The current internal damage value. Used for knockback calculations.
        /// </summary>
        public float CurrentDamage { get; set; }
        public float DefaultDamage { get; set; }

        public DamageType DamageType { get; set; }
        public ModifierGroup<object> DamageModifiers { get; private set; }
        public ModifierGroup<object> HealingModifiers { get; private set; }

        internal float ModifyDamage(float baseDamage, object source = null) {
            return DamageModifiers.Out.Modifiy(source, baseDamage);
        }

        public void Damage(float damage) {
            Damage(null, damage);
        }
        
        public void Damage(object source, float damage) {
            damage = Mathf.Abs(damage);

            if (DamageModifiers.In.Count > 0)
                damage = DamageModifiers.In.Modifiy(source, damage);

            CurrentDamage = DamageType.Damage(CurrentDamage, damage);

            Mathf.Clamp(CurrentDamage, DamageType.MinDamage, DamageType.MaxDamage);

            CharacterEvents.Publish(new PlayerDamageEvent {damage = damage, currentDamage = CurrentDamage});
        }

        public void Heal(float healing) {
           Heal(null, healing); 
        }

        public void Heal(object source, float healing) {
            healing = Mathf.Abs(healing);

            if (HealingModifiers.In.Count > 0)
                healing = HealingModifiers.In.Modifiy(source, healing);

            CurrentDamage = DamageType.Heal(CurrentDamage, healing);

            CharacterEvents.Publish(new PlayerHealEvent { healing = healing, currentDamage = CurrentDamage });
        }

    }

}
