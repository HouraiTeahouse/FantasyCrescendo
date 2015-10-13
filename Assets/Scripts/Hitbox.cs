using System;
using UnityEngine;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    [RequireComponent(typeof (Collider))]
    public sealed class Hitbox : MonoBehaviour, IDamager {

        public static readonly string Tag = "Hitbox";
        public const int HitboxLayer = 10;
        public const int HurtboxLayer = 11;

        public enum Type {

            Offensive,
            Damageable,
            Invincible,
            Intangible,
            Shield,
            Absorb,
            Reflective

        }

        private ParticleSystem _effect;
        private AudioSource _soundEffect;
        private Collider[] colliders;

        // Represents the source Character that owns this Hitbox
        // If this is a Offensive type hitbox, this ensures that the Character doesn't damage themselves
        // If this is a Damageable type Hitbox (AKA a Hurtbox) this is the character that the damage and knockback is applied to.
        public Character Source { get; set; }

        private IDamageable _damageable;
        private IKnockbackable _knockbackable;

        public IDamageable Damageable {
            get {
                return _damageable;
            }
        }

        public IKnockbackable Knockbackable {
            get {
                return _knockbackable;
            }
        }

        private void Awake() {
            Source = GetComponentInParent<Character>();
            if(Source) {
                _damageable = Source;
                _knockbackable = Source;
            } else {
               _damageable = GetComponentInParent<IDamageable>();
                _knockbackable = GetComponentInParent<IKnockbackable>();
            }
            _effect = GetComponent<ParticleSystem>();
            _soundEffect = GetComponent<AudioSource>();

            gameObject.tag = Tag;
            switch(type) {
                case Type.Damageable:
                case Type.Shield:
                    gameObject.layer = HurtboxLayer;
                    break;
                default:
                    gameObject.layer = HitboxLayer;
                    break;
            }

            colliders = GetComponents<Collider>();
            for (var i = 0; i < colliders.Length; i++)
                colliders[i].isTrigger = true;
        }

        #region Unity Callbacks
         void OnDrawGizmos() {
            GizmoUtil.DrawColliders3D(GetComponentsInChildren<Collider>(), SmashGame.GetHitboxColor(type), true);
        }
        
        void OnTriggerEnter(Collider other) {
            if (!other.CompareTag(Tag))
                return;
            Hitbox otherHitbox = other.GetComponent<Hitbox>();
            if (otherHitbox == null)
                return;
            
            switch (otherHitbox.type) {
                case Type.Damageable:
                    switch (type) {
                        case Type.Damageable:
                            Debug.Log("Two hurtboxes should not collide with each other.");
                            break;
                        case Type.Offensive:
                            if(otherHitbox.Damageable != null)
                                otherHitbox.Damageable.Damage(this);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Serializable Fields

        [SerializeField]
        private Type type;

        [SerializeField]
        private int _priority = 100;

        [SerializeField]
        private float _damage = 5f;

        [SerializeField]
        private float _angle = 45f;

        [SerializeField]
        private float _baseKnockback;

        [SerializeField]
        private float _knockbackScaling;

        [SerializeField]
        private bool _reflectable;

        [SerializeField]
        private bool _absorbable;

        #endregion

        #region Public Access Properties
        public int Priority {
            get { return _priority; }
            set { _priority = value; }
        }

        public float Damage {
            get { return _damage; }
            set { _damage = value; }
        }

        public float Angle {
            get { return _angle; }
            set { _angle = value; }
        }

        public float BaseKnockback {
            get { return _baseKnockback; }
            set { _baseKnockback = value; }
        }

        public float KnockbackScaling {
            get { return _knockbackScaling; }
            set { _knockbackScaling = value; }
        }

        public bool Reflectable {
            get { return _reflectable; }
            set { _reflectable = value; }
        }

        public bool Absorbable {
            get { return _absorbable; }
            set { _absorbable = value; }
        }

        public float BaseDamage {
            get {
                return Source == null ? _damage : Source.ModifyDamage(_damage);
            }
        }

        #endregion
    }

}