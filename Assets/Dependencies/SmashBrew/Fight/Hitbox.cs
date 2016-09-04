using System;
using UnityConstants;
using UnityEngine;
using Random = System.Random;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace HouraiTeahouse.SmashBrew {

    [DisallowMultipleComponent]
    public sealed class Hitbox : MonoBehaviour {

        public enum Type {

            // The values here are used as priority mulitpliers
            Inactive = 1,
            Offensive = Inactive << 1,
            Damageable = Offensive << 1,
            Invincible = Damageable << 1,
            Intangible = Invincible << 1,
            Shield = Intangible << 1,
            Absorb = Shield << 1,
            Reflective = Absorb << 1

        }

        static readonly Table2D<Type, Action<Hitbox, Hitbox>> ReactionMatrix;

        [SerializeField]
        [ReadOnly]
        int _id;

        [SerializeField]
        [HideInInspector]
        Mesh _capsule;

        //TODO: Add triggers for on hit effects and SFX
        //ParticleSystem _effect;
        //AudioSource _soundEffect;
        Collider[] _colliders;

        [SerializeField]
        [HideInInspector]
        Mesh _cube;

        IRegistrar<Hitbox> _registrar;

        [SerializeField]
        [HideInInspector]
        Material _material;

        [SerializeField]
        [HideInInspector]
        Mesh _sphere;

        static Hitbox() {
            ReactionMatrix = new Table2D<Type, Action<Hitbox, Hitbox>>();
            ReactionMatrix[Type.Offensive, Type.Damageable] = delegate(Hitbox src, Hitbox dst) {
                if (dst.Damageable != null)
                    dst.Damageable.Damage(src, src.BaseDamage);
                if (dst.Knockbackable != null)
                    //TODO : FIX
                    dst.Knockbackable.Knockback(src, Vector2.one);
                DrawEffect(src, dst);
            };
            ReactionMatrix[Type.Offensive, Type.Absorb] = ExecuteInterface<IAbsorbable>(h => h.Absorbable,
                (a, o) => a.Absorb(o));
            ReactionMatrix[Type.Offensive, Type.Reflective] = ExecuteInterface<IReflectable>(h => h.Reflectable,
                (a, o) => a.Reflect(o));
            ReactionMatrix[Type.Offensive, Type.Invincible] = DrawEffect;
        }

        /// <summary> Whether hitboxes should be drawn or not. </summary>
        public static bool DrawHitboxes { get; set; }

        // Represents the source Character that owns this Hitbox
        // If this is a Offensive type hitbox, this ensures that the Character doesn't damage themselves
        // If this is a Damageable type Hitbox (AKA a Hurtbox) this is the character that the damage and knockback is applied to.
        public Character Source { get; set; }

        public IDamageable Damageable { get; private set; }

        public IKnockbackable Knockbackable { get; private set; }

        public IRegistrar<Hitbox> Registrar {
            get { return _registrar; }
            set {
                if (_registrar != null)
                    _registrar.Unregister(this);
                _registrar = value;
                if (value != null)
                    value.Register(this);
            }
        }

        static Action<Hitbox, Hitbox> ExecuteInterface<T>(Predicate<Hitbox> check, Action<T, object> action) {
            return delegate(Hitbox src, Hitbox dst) {
                if (!check(src))
                    return;
                foreach (T component in src.GetComponents<T>())
                    action(component, dst);
            };
        }

        static void DrawEffect(Hitbox src, Hitbox dst) { throw new NotImplementedException(); }

        public static void Resolve(Hitbox src, Hitbox dst) {
            ReactionMatrix[src.DefaultType, dst.DefaultType](src, dst);
        }

        #region Unity Callbacks

        /// <summary> Unity callback. Called on object instantiation. </summary>
        void Awake() {
            CurrentType = DefaultType;
            Registrar = GetComponentInParent<IRegistrar<Hitbox>>();
            Source = GetComponentInParent<Character>();
            Damageable = GetComponentInParent<IDamageable>();
            Knockbackable = GetComponentInParent<IKnockbackable>();
            //_effect = GetComponent<ParticleSystem>();
            //_soundEffect = GetComponent<AudioSource>();

            gameObject.tag = Tags.Hitbox;
            switch (CurrentType) {
                case Type.Damageable:
                case Type.Shield:
                    gameObject.layer = Layers.Hurtbox;
                    break;
                default:
                    gameObject.layer = Layers.Hitbox;
                    break;
            }
            _colliders = GetComponents<Collider>();
            foreach (Collider col in _colliders)
                col.isTrigger = true;
        }

#if UNITY_EDITOR
        bool gizmoInitialized;

        void OnDrawGizmos() {
            if (!EditorApplication.isPlayingOrWillChangePlaymode && !gizmoInitialized) {
                ResetType();
                gizmoInitialized = true;
            }
            if (IsActive)
                Gizmo.DrawColliders(GetComponents<Collider>(), Config.Debug.GetHitboxColor(CurrentType));
        }
#endif

        void OnRenderObject() {
            if (!DrawHitboxes)
                return;
            if (_colliders == null)
                _colliders = GetComponents<Collider>();
            Color color = Config.Debug.GetHitboxColor(CurrentType);
            foreach (Collider col in _colliders)
                DrawCollider(col, color);
            //GL.wireframe = true;
            //foreach (var col in _colliders)
            //    DrawCollider(col, Color.white);
            //GL.wireframe = false;
        }

        void Reset() { _id = new Random().Next(int.MaxValue); }

        void DrawCollider(Collider col, Color color) {
            if (col == null)
                return;
            Mesh mesh = null;
            if (col is SphereCollider)
                mesh = _sphere;
            else if (col is BoxCollider)
                mesh = _cube;
            else if (col is CapsuleCollider)
                mesh = _capsule;
            else if (col is MeshCollider)
                mesh = ((MeshCollider) col).sharedMesh;
            if (mesh == null)
                return;
            _material.SetColor("_Color", color);
            _material.SetPass(0);
            Graphics.DrawMeshNow(mesh, Gizmo.GetColliderMatrix(col));
        }

        void OnTriggerEnter(Collider other) {
            if (!other.CompareTag(Tags.Hitbox))
                return;
            var otherHitbox = other.GetComponent<Hitbox>();
            if (otherHitbox == null || !ReactionMatrix.ContainsKey(CurrentType, otherHitbox.CurrentType))
                return;
            HitboxResolver.AddCollision(this, otherHitbox);
        }

        public bool ResetType() {
            bool val = CurrentType != DefaultType;
            CurrentType = DefaultType;
            return val;
        }

        #endregion

        #region Serializable Fields

        Type currentType;

        [SerializeField]
        Type type = Type.Offensive;

        [SerializeField]
        int _priority = 100;

        [SerializeField]
        float _damage = 5f;

        [SerializeField]
        float _angle = 45f;

        [SerializeField]
        float _baseKnockback;

        [SerializeField]
        float _knockbackScaling;

        [SerializeField]
        bool _reflectable;

        [SerializeField]
        bool _absorbable;

        #endregion

        #region Public Access Properties

        public int ID {
            get { return _id; }
            set { _id = value; }
        }

        public bool IsActive {
            get { return CurrentType != Type.Inactive; }
        }

        public Type DefaultType {
            get { return type; }
            set { type = value; }
        }

        public Type CurrentType { get; set; }

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

        public float Scaling {
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
            get { return Source == null ? _damage : Source.GetComponent<PlayerDamage>().ModifyDamage(_damage); }
        }

        public bool FlipDirection {
            get {
                //TODO: Implement properly
                return false;
            }
        }

        #endregion
    }

}
