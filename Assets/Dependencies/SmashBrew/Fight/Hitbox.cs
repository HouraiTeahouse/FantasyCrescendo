using System;
using System.Collections.Generic;
using HouraiTeahouse.SmashBrew.Characters;
using UnityEngine;
using Random = System.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.SmashBrew {

    [DisallowMultipleComponent]
    public sealed class Hitbox : MonoBehaviour, IResettable {

        static bool _initialized = false;

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
        static readonly List<Hitbox> _hitboxes;

        public static IEnumerable<Hitbox> ActiveHitboxes {
            get { return _hitboxes; }
        }

        [SerializeField]
        [ReadOnly]
        int _id;

        //TODO: Add triggers for on hit effects and SFX
        //ParticleSystem _effect;
        //AudioSource _soundEffect;
        Collider[] _colliders;

        HashSet<object> _history;

        IRegistrar<Hitbox> _registrar;

        Type currentType;

        [SerializeField]
        Type type = Type.Offensive;

        [SerializeField]
        int _priority = 100;

        [SerializeField]
        float _damage = 5f;

        [SerializeField, Range(0, 360)]
        float _angle = 45f;

        [SerializeField]
        float _baseKnockback;

        [SerializeField]
        float _knockbackScaling = 1f;

        [SerializeField]
        bool _reflectable;

        [SerializeField]
        bool _absorbable;
        
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
            get { return Source == null ? _damage : Source.GetComponent<DamageState>().ModifyDamage(_damage); }
        }

        public bool FlipDirection {
            get {
                //TODO: Implement properly
                return false;
            }
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

        static Hitbox() {
            ReactionMatrix = new Table2D<Type, Action<Hitbox, Hitbox>>();
            _hitboxes = new List<Hitbox>();
            ReactionMatrix[Type.Offensive, Type.Damageable] = delegate(Hitbox src, Hitbox dst) {
                if (dst.Damageable != null)
                    dst.Damageable.Damage(src, src.BaseDamage);
                if (dst.Knockbackable != null) {
                    var angle = Mathf.Deg2Rad * src.Angle;
                    dst.Knockbackable.Knockback(src, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)));
                }
                DrawEffect(src, dst);
            };
            ReactionMatrix[Type.Offensive, Type.Absorb] = ExecuteInterface<IAbsorbable>(h => h.Absorbable,
                (a, o) => a.Absorb(o));
            ReactionMatrix[Type.Offensive, Type.Reflective] = ExecuteInterface<IReflectable>(h => h.Reflectable,
                (a, o) => a.Reflect(o));
            ReactionMatrix[Type.Offensive, Type.Invincible] = DrawEffect;
        }

        static Action<Hitbox, Hitbox> ExecuteInterface<T>(Predicate<Hitbox> check, Action<T, object> action) {
            return delegate(Hitbox src, Hitbox dst) {
                if (!check(src))
                    return;
                foreach (T component in src.GetComponents<T>())
                    action(component, dst);
            };
        }

       static void DrawEffect(Hitbox src, Hitbox dst) { 
            // TODO(james7132): Implement
        }

        public static void Resolve(Hitbox src, Hitbox dst) {
            ReactionMatrix[src.DefaultType, dst.DefaultType](src, dst);
        }

        void Initialize() {
            if (_initialized)
                return;
            // Draw Hitboxes in Debug builds
            DrawHitboxes = Debug.isDebugBuild;
            _initialized = true;
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            Initialize();
            CurrentType = DefaultType;
            Registrar = GetComponentInParent<IRegistrar<Hitbox>>();
            Source = GetComponentInParent<Character>();
            Damageable = GetComponentInParent<IDamageable>();
            Knockbackable = GetComponentInParent<IKnockbackable>();
            _history = new HashSet<object>();
            //_effect = GetComponent<ParticleSystem>();
            //_soundEffect = GetComponent<AudioSource>();

            gameObject.tag = Config.Tags.HitboxTag;
            switch (CurrentType) {
                case Type.Damageable:
                case Type.Shield:
                    gameObject.layer = Config.Tags.HurtboxLayer;
                    break;
                default:
                    gameObject.layer = Config.Tags.HitboxLayer;
                    break;
            }
            _colliders = GetComponents<Collider>();
            foreach (Collider col in _colliders)
                col.isTrigger = true;
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable() {
            _hitboxes.Add(this); 
            _history.Clear();
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable() {
            _hitboxes.Remove(this); 
            _history.Clear();
        }

#if UNITY_EDITOR
        bool gizmoInitialized;

        /// <summary>
        /// Callback to draw gizmos that are pickable and always drawn.
        /// </summary>
        void OnDrawGizmos() {
            if (!EditorApplication.isPlayingOrWillChangePlaymode && !gizmoInitialized) {
                ResetState();
                gizmoInitialized = true;
            }
            if (IsActive)
                Gizmo.DrawColliders(GetComponents<Collider>(), Config.Debug.GetHitboxColor(CurrentType));
        }
#endif

        public bool CheckHistory(object obj) {
            var result = _history.Contains(obj);
            if (!result)
                _history.Add(obj);
            if (Source != null) {
                var charHistory = Source.CheckHistory(obj);
                return result || charHistory;
            }
            return result;
        }

        public void ClearHistory() {
            if (_history != null)
                _history.Clear();
        }

        public void DrawHitbox() {
            if (!DrawHitboxes)
                return;
            if (_colliders == null)
                _colliders = GetComponents<Collider>();
            Color color = Config.Debug.GetHitboxColor(CurrentType);
            foreach (Collider col in _colliders)
                DrawCollider(col, color);
        }

        /// <summary>
        /// Reset is called when the user hits the Reset button in the Inspector's
        /// context menu or when adding the component the first time.
        /// </summary>
        void Reset() {
            _id = new Random().Next(int.MaxValue); 
        }

        void DrawCollider(Collider col, Color color) {
            if (col == null)
                return;
            Mesh mesh = null;
            if (col is SphereCollider)
                mesh = PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Sphere);
            else if (col is BoxCollider)
                mesh = PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Cube);
            else if (col is CapsuleCollider)
                mesh = PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Capsule);
            else if (col is MeshCollider)
                mesh = ((MeshCollider) col).sharedMesh;
            if (mesh == null)
                return;
            Config.Debug.HitboxMaterial.SetColor("_Color", color);
            Config.Debug.HitboxMaterial.SetPass(0);
            Graphics.DrawMeshNow(mesh, Gizmo.GetColliderMatrix(col));
        }

        /// <summary>
        /// OnTriggerEnter is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        void OnTriggerEnter(Collider other) { 
            if (!other.CompareTag(Config.Tags.HitboxTag))
                return;
            var otherHitbox = other.GetComponent<Hitbox>();
            if (otherHitbox == null || !ReactionMatrix.ContainsKey(CurrentType, otherHitbox.CurrentType))
                return;
            if (CheckHistory(other) || CheckHistory(otherHitbox) || CheckHistory(otherHitbox.Damageable) || CheckHistory(otherHitbox.Knockbackable))
                return;
            HitboxResolver.AddCollision(this, otherHitbox);
        }

        public bool ResetState() {
            bool val = CurrentType != DefaultType;
            CurrentType = DefaultType;
            ClearHistory();
            return val;
        }

        void IResettable.OnReset() {
            ResetState();
        }

    }

}
