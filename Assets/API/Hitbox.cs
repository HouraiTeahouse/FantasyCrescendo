using UnityEngine;
using System.Collections;

namespace Crescendo.API {
    
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SphereCollider))]
    public sealed class Hitbox : MonoBehaviour {

        #region Serializable Fields
        [SerializeField]
        private HitboxType type;

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
        #endregion

        // Represents the source Character that owns this Hitbox
        // If this is a Offensive type hitbox, this ensures that the Character doesn't damage themselves
        // If this is a Damageable type Hitbox (AKA a Hurtbox) this is the character that the damage and knockback is applied to.
        private Character _source;
        private ParticleSystem _effect;
        private AudioSource _soundEffect;

        void Awake() {
            _source = GetComponentInParent<Character>();
            _effect = GetComponent<ParticleSystem>();
            _soundEffect = GetComponent<AudioSource>();
        }

        void OnDrawGizmos() {
            GizmoUtil.DrawColliders3D(GetComponentsInChildren<Collider>(), Game.GetHitboxColor(type), true);
        }

    }

}
