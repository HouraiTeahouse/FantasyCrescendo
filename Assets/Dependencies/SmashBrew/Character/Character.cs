using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hourai.SmashBrew {

    /// <summary>
    /// General character class for handling the physics and animations of individual characters
    /// </summary>
    /// Author: James Liu
    /// Authored on 07/01/2015
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    [RequireComponent(typeof(NetworkAnimator), typeof(NetworkTransform))]
    public partial class Character : HouraiNetworkBehaviour {

        private static readonly Type[] RequiredComponents;

        private const int PlayerLayer = 9;

        private enum FacingMode {
            Rotation,
            Scale
        }

        static Character() {
            var componentType = typeof(Component);
            var requiredComponentType = typeof(RequiredCharacterComponentAttribute);
            // Use reflection to find required Components for Characters and statuses
            // Enumerate all concrete Component types
            RequiredComponents = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                  from assemblyType in domainAssembly.GetTypes()
                                  where
                                      assemblyType != null &&
                                      !assemblyType.IsAbstract &&
                                      componentType.IsAssignableFrom(assemblyType) &&
                                      assemblyType.IsDefined(requiredComponentType, true)
                                  select assemblyType).ToArray();
        }

        #region Public Properties
        /// <summary>
        /// The direction the character is currently facing.
        /// If set to true, the character faces the right.
        /// If set to false, the character faces the left.
        /// 
        /// The method in which the character is flipped depends on what the Facing Mode parameter is set to.
        /// </summary>
        public bool Facing {
            get {
                if (_facingMode == FacingMode.Rotation)
                    return eulerAngles.y > 179f;
                return localScale.x > 0;
            }
            set {
                if (_facing == value)
                    return;

                _facing = value;
                if (_facingMode == FacingMode.Rotation)
                    Rotate(0f, 180f, 0f);
                else
                    localScale *= -1;
            }
        }

        public bool IsHit {
            get { return _hitstun > 0; }
        }

        public Player Player { get; internal set; }

        public bool IsGrounded {
            get { return Animator.GetBool(CharacterAnimVars.Grounded); }
            private set {
                if (IsGrounded == value)
                    return;
                Animator.SetBool(CharacterAnimVars.Grounded, value);
                if (OnGrounded != null)
                    OnGrounded();
            }
        }

        public bool IsDashing {
            get { return IsGrounded && _isDashing; }
            set { _isDashing = value; }
        }

        public ModifierList DamageDealt {
            get; private set;
        }

        public ModifierList KnockbackDealt {
            get; private set;
        }

        public ModifierList<IKnockbacker> KnockbackTaken {
            get; private set;
        }

        public int BoneCount {
            get { return _bones.Length; }
        }
        #endregion

        #region Runtime Variables
        private bool _facing;
        private HashSet<CharacterComponent> components;
        private HashSet<Collider> ground;
        private Transform[] _bones;
        #endregion

        #region Serialized Variables
        [SerializeField]
        private FacingMode _facingMode;

        [SerializeField]
        private GameObject _rootBone;
        #endregion

        #region Public Events
        [SyncEvent]
        public event Action OnGrounded;

        [SyncEvent]
        public event Action OnJump;

        [SyncEvent]
        public event Action<Attack.Type, Attack.Direction, int> OnAttack;
        #endregion

        void AttachRequiredComponents() {
            foreach (Type requriedType in RequiredComponents)
                if(gameObject.GetComponent(requriedType) == null)
                    gameObject.AddComponent(requriedType);
        }

        #region Required Components
        public CapsuleCollider MovementCollider { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public Animator Animator { get; private set; }
        #endregion

        #region State Variables
        private Vector3 _oldVelocity;
        private float _hitstun;
        private bool _isDashing;
        #endregion
        
        #region Public Action Methods
        public void Attack(Attack.Type type, Attack.Direction direction, int index) {
            if(OnAttack != null)
                OnAttack(type, direction, index);
        }

        public void Knockback(IKnockbacker source) {
        }
        
        public Transform GetBone(int boneIndex) {
            if (boneIndex < 0 || boneIndex >= BoneCount)
                return transform;
            return _bones[boneIndex];
        }
        #endregion

        #region Internal Methods
        internal float ModifyDamage(float baseDamage) {
            if (DamageDealt.Count <= 0)
                return baseDamage;
            return DamageDealt.Modifiy(baseDamage);
        }
        #endregion

        #region Unity Callbacks
        protected virtual void Awake() {
            gameObject.tag = SmashGame.Config.PlayerTag;
            
            ground = new HashSet<Collider>();
            
            KnockbackTaken = new ModifierList<IKnockbacker>();

            DamageDealt = new ModifierList();
            KnockbackDealt = new ModifierList();

            MovementCollider = GetComponent<CapsuleCollider>();
            MovementCollider.isTrigger = false;

            gameObject.layer = PlayerLayer;

            Rigidbody = GetComponent<Rigidbody>();
            Rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

            Animator = GetComponent<Animator>();
            Animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

            AttachRequiredComponents();

            _bones = (_rootBone ?? gameObject).GetComponentsInChildren<Transform>();

            // Initialize all animation behaviours
            BaseAnimationBehaviour.InitializeAll(Animator);
        }

        protected virtual void OnEnable() {
            // TODO: Find a better place to put this
            CameraController.AddTarget(this);
            foreach (var comp in components)
                comp.enabled = true;
        }

        protected virtual void OnDisable() {
            // TODO: Find a better place to put this
            CameraController.RemoveTarget(this);
            foreach (var comp in components)
                comp.enabled = false;
        }

        void Reset() {
            AttachRequiredComponents();
        }

        /// <summary>
        /// Called every physics update.
        /// </summary>
        protected virtual void FixedUpdate() {
            float dt = Time.fixedDeltaTime;

            _oldVelocity = Rigidbody.velocity;

            if (IsHit)
                _hitstun -= dt;
        }

        protected virtual void OnAnimatorMove() {
            //TODO: Merge Physics and Animation Movements here

            //_rigidbody.velocity = _animator.deltaPosition / Time.deltaTime;
        }

        protected virtual void OnCollisionEnter(Collision col) {
            ContactPoint[] points = col.contacts;
            if (points.Length <= 0)
                return;

            if (IsHit) {
                Vector3 point = points[0].point;
                Vector3 normal = points[0].normal;
                Vector3 reflection = _oldVelocity - 2 * Vector2.Dot(_oldVelocity, normal) * normal;
                Debug.DrawRay(point, reflection, Color.green);
                Debug.DrawRay(point, normal, Color.red);
                Debug.DrawRay(point, -_oldVelocity, Color.yellow);
                Rigidbody.velocity = Vector3.ClampMagnitude(reflection, 0.8f * _oldVelocity.magnitude);
            }

            float r2 = MovementCollider.radius * MovementCollider.radius;
            Vector3 bottom = transform.TransformPoint(MovementCollider.center - Vector3.up * MovementCollider.height / 2);
            for (var i = 0; i < points.Length; i++)
                if ((points[i].point - bottom).sqrMagnitude < r2)
                    ground.Add(points[i].otherCollider);
            IsGrounded = ground.Count > 0;
        }

        protected virtual void OnCollisionExit(Collision col) {
            if(ground.Remove(col.collider))
                IsGrounded = ground.Count > 0;
        }
        #endregion
    }

}