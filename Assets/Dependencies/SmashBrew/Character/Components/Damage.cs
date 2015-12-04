using Hourai.Events;
using UnityEngine;

namespace Hourai.SmashBrew {

    public class DamageEvent : IEvent {

        public float damage;
        public float currentDamage;

    }

    public class HealEvent : IEvent {

        public float healing;
        public float currentDamage;

    }

    [DisallowMultipleComponent]
    public class Damage : CharacterComponent, IDamageable, IHealable {

        [SerializeField]
        private float _currentDamage = 0f;

        [SerializeField]
        private float _defaultDamage = 0f;

        public float CurrentDamage {
            get { return _currentDamage; }
            set { _currentDamage = value; }
        }

        public float DefaultDamage {
            get { return _defaultDamage; }
            set { _defaultDamage = value; }
        }

        public string Suffix {
            get { return "%"; }
        }

        public float MinDamage { get; protected set; }
        public float MaxDamage { get; protected set; }

        public ModifierList<object> DefensiveModifiers {
            get; private set;
        }

        public ModifierList<object> HealingModifiers {
            get; private set;
        }

        void Awake() {
            MinDamage = 0f;
            MaxDamage = 999.9999f;
            DefensiveModifiers = new ModifierList<object>();
            HealingModifiers = new ModifierList<object>();
        }
        
        public void Hurt(object source, float damage) {
            if (!enabled)
                return;

            damage = Mathf.Abs(damage);

            if (DefensiveModifiers.Count > 0)
                damage = DefensiveModifiers.Modifiy(source, damage);

            HurtImpl(damage);

            Mathf.Clamp(CurrentDamage, MinDamage, MaxDamage);

            CharacterEvents.Publish(new DamageEvent {damage = damage, currentDamage = CurrentDamage});
        }

        protected virtual void HurtImpl(float damage) {
            CurrentDamage += damage;
            if (CurrentDamage > MaxDamage)
                CurrentDamage = MaxDamage;
        }

        public virtual void Heal(object source, float healing) {
            if (!enabled)
                return;

            healing = Mathf.Abs(healing);

            if (HealingModifiers.Count > 0)
                healing = HealingModifiers.Modifiy(source, healing);

            HealImpl(healing);

            CharacterEvents.Publish(new HealEvent { healing = healing, currentDamage = CurrentDamage });

        }

        protected virtual void HealImpl(float damage) {
            CurrentDamage += damage;
            if (CurrentDamage > MaxDamage)
                CurrentDamage = MaxDamage;
        }

        void IDamageable.Damage(object source, float damage) {
            Hurt(source, damage);
        }

        public void Reset() {
            CurrentDamage = DefaultDamage;
        }
    }

}