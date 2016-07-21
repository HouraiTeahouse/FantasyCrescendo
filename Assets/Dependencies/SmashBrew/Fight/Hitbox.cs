// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using UnityConstants;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    [DisallowMultipleComponent]
    public sealed class Hitbox : MonoBehaviour {

        [Flags]
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
            ReactionMatrix[Type.Offensive, Type.Damageable] =
                delegate(Hitbox src, Hitbox dst) {
                    if (dst.Damageable != null)
                        dst.Damageable.Damage(src, src.BaseDamage);
                    if (dst.Knockbackable != null)
                        //TODO : FIX
                        dst.Knockbackable.Knockback(src, Vector2.one);
                    DrawEffect(src, dst);
                };
            ReactionMatrix[Type.Offensive, Type.Absorb] =
                ExecuteInterface<IAbsorbable>(h => h.Absorbable,
                    (a, o) => a.Absorb(o));
            ReactionMatrix[Type.Offensive, Type.Reflective] =
                ExecuteInterface<IReflectable>(h => h.Reflectable,
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
                if(value != null)
                    value.Register(this);
            }
        }

        static Action<Hitbox, Hitbox> ExecuteInterface<T>(
            Predicate<Hitbox> check,
            Action<T, object> action) {
            return delegate(Hitbox src, Hitbox dst) {
                if (!check(src))
                    return;
                T[] components = src.GetComponents<T>();
                for (var i = 0; i < components.Length; i++)
                    action(components[i], dst);
            };
        }

        static void DrawEffect(Hitbox src, Hitbox dst) {
            throw new NotImplementedException();
        }

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
            if (!gizmoInitialized) {
                CurrentType = DefaultType;
                gizmoInitialized = true;
            }
            if(IsActive)
                GizmoUtil.DrawColliders(GetComponents<Collider>(),
                    Config.Debug.GetHitboxColor(CurrentType));
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

        void Reset() { _id = new System.Random().Next(int.MaxValue);}

        void DrawCollider(Collider col, Color color) {
            if (col == null)
                return;
            Mesh mesh = null;
            var boxCol = col as BoxCollider;
            var sphereCol = col as SphereCollider;
            var capsuleCol = col as CapsuleCollider;
            Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;
            Vector3 scale = Vector3.one;
            Matrix4x4 localToWorld;
            if (boxCol != null) {
                mesh = _cube;
                position = boxCol.center;
                scale = boxCol.size;
                localToWorld = transform.localToWorldMatrix;
            }
            else if (sphereCol != null) {
                mesh = _sphere;
                position = sphereCol.center;
                scale = sphereCol.radius * Vector3.one;
                localToWorld = Matrix4x4.TRS(transform.position,
                    transform.rotation,
                    Vector3.one * transform.lossyScale.Max());
            }
            else if (capsuleCol != null) {
                mesh = _capsule;
                position = capsuleCol.center;
                scale = Vector3.one * capsuleCol.radius * 2;
                scale[capsuleCol.direction] = capsuleCol.height / 2;
                switch (capsuleCol.direction) {
                    case 1:
                        rotation = Quaternion.Euler(0, 90, 0);
                        break;
                    case 2:
                        rotation = Quaternion.Euler(90, 0, 0);
                        break;
                }
                localToWorld = transform.localToWorldMatrix;
            }
            else {
                localToWorld = transform.localToWorldMatrix;
            }
            _material.SetColor("_Color", color);
            _material.SetPass(0);
            Graphics.DrawMeshNow(mesh,
                localToWorld * Matrix4x4.TRS(position, rotation, scale));
        }

        void OnTriggerEnter(Collider other) {
            if (!other.CompareTag(Tags.Hitbox))
                return;
            var otherHitbox = other.GetComponent<Hitbox>();
            if (otherHitbox == null
                || !ReactionMatrix.ContainsKey(CurrentType, otherHitbox.CurrentType))
                return;
            HitboxResolver.AddCollision(this, otherHitbox);
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
            get {
                return Source == null
                    ? _damage
                    : Source.GetComponent<PlayerDamage>().ModifyDamage(_damage);
            }
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
