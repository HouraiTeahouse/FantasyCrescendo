using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public class Damage : MonoBehaviour, IDamageable, IHealable {

        [SerializeField, SyncVar]
        private float _currentDamage = 0f;

        [SerializeField, SyncVar]
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

        [SyncEvent]
        public event Action<object, float> OnDamage;

        [SyncEvent]
        public event Action<object, float> OnHeal;

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

            if (OnDamage != null)
                OnDamage(source, damage);
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

            if (OnHeal != null)
                OnHeal(source, healing);
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