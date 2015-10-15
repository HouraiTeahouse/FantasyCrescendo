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
    public partial class Character : HouraiBehaviour, IDamageable, IKnockbackable, IHealable {

        private static readonly Type[] RequiredComponents;

        private const int PlayerLayer = 9;

        private static readonly int _animGrounded = Animator.StringToHash("grounded");
        private static readonly int _animHInput = Animator.StringToHash("horizontal input");
        private static readonly int _animVInput = Animator.StringToHash("vertical input");
        private static readonly int _animAttack = Animator.StringToHash("attack");
        private static readonly int _animSpecial = Animator.StringToHash("special");
        private static readonly int _animJump = Animator.StringToHash("jump");

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

        public float Gravity {
            get { return _gravity; }
            set { _gravity = Mathf.Abs(value); }
        }

        public Player Player { get; internal set; }
        public ICharacterInput InputSource { get; set; }

        public bool IsGrounded {
            get { return _isGrounded; }
            private set {
                if (IsGrounded == value)
                    return;
                _isGrounded = value;
                Animator.SetBool(_animGrounded, value);
                if (_isGrounded)
                    JumpCount = 0;
                if (OnGrounded != null)
                    OnGrounded();
            }
        }

        public int JumpCount { get; private set; }

        public int MaxJumpCount {
            get { return _jumpHeights == null ? 0 : _jumpHeights.Length; }
        }

        public bool IsDashing {
            get { return IsGrounded && _isDashing; }
            set { _isDashing = value; }
        }
        
        /// <summary>
        /// A dynamically constructed data structure handling the 
        /// </summary>
        public ModifierList<IHealer> Healing {
            get; private set;
        }

        public ModifierList DamageDealt {
            get; private set;
        }

        public ModifierList KnockbackDealt {
            get; private set;
        }

        public ModifierList<IDamager> DamageTaken {
            get; private set;
        }

        public ModifierList<IKnockbacker> KnockbackTaken {
            get; private set;
        }

        private CharacterDamage Damageable {
            get {
                if (!_damageable)
                    _damageable = GetComponent<CharacterDamage>();
                return _damageable;
            }
        }
        #endregion

        #region Runtime Variables
        private bool _facing;
        private HashSet<CharacterComponent> components;
        private HashSet<Collider> ground;
        #endregion

        #region Serialized Variables
        [SerializeField]
        private float _gravity = 9.86f;

        [SerializeField]
        private float[] _jumpHeights = { 1.5f, 1.5f };

        [SerializeField]
        private FacingMode _facingMode;
        #endregion

        #region Public Events
        public event Action OnGrounded;
        public event Action OnAttack;
        public event Action OnSpecial;
        public event Action OnJump;
        public event Action<IDamager, float> OnDamage;
        public event Action<IHealer, float> OnHeal;
        #endregion

        void AttachRequiredComponents() {
            foreach (Type requriedType in RequiredComponents)
                if(gameObject.GetComponent(requriedType) == null)
                    gameObject.AddComponent(requriedType);
        }

        #region Required Components
        private CharacterDamage _damageable;
        
        public CapsuleCollider MovementCollider { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public Animator Animator { get; private set; }
        #endregion

        #region State Variables
        private bool _isGrounded;
        private bool _isDashing;
        #endregion

        #region Physics Properties
        public Vector3 Velocity {
            get { return Rigidbody.velocity; }
            set { Rigidbody.velocity = value; }
        }

        public float Mass {
            get { return Rigidbody.mass; }
            set { Rigidbody.mass = value; }
        }
        #endregion

        #region Public Action Methods
        public void Attack() {
            //if (Restricted)
            //    return;

            Animator.SetTrigger(_animAttack);
            
            if(OnAttack != null)
                OnAttack();
        }

        public void Special() {
            //if (Restricted)
            //    return;

            Animator.SetTrigger(_animSpecial);

            if (OnSpecial != null)
                OnSpecial();
        }
        
        public void Jump() {
            if (JumpCount >= MaxJumpCount)//Restricted)
                return;

            float g = Gravity;

            // Apply upward force to jump
            Vector3 temp = Velocity;
            temp.y = Mathf.Sqrt(2 * g * _jumpHeights[JumpCount]);
            Velocity = temp;

            JumpCount++;

            // Trigger animation
            Animator.SetTrigger(_animJump);

            if (OnJump != null)
                OnJump();
        }

        public T ApplyStatus<T>(float duration = -1f) where T : Status {
            T instance = GetComponent<T>();
            if(instance == null)
                instance = gameObject.AddComponent<T>();
            instance.StartStatus(duration);
            return instance;
        }

        public void Damage(IDamager source) {
            if (source == null || Damageable == null)
                return;

            float damage = Mathf.Abs(source.BaseDamage);

            if (DamageTaken.Count > 0)
                damage = DamageTaken.Modifiy(source, damage);

            _damageable.Damage(source, damage);

            if (OnDamage != null)
                OnDamage(source, damage);
        }

        public void Knockback(IKnockbacker source) {
            if (source == null)
                return;

            float angle = Mathf.Deg2Rad * source.Angle;
            float damage = (Damageable != null) ? Damageable.CurrentDamage : 0f;

            float knockback = source.BaseKnockback + source.Scaling * damage;

            if (KnockbackTaken.Count > 0)
                knockback = KnockbackTaken.Modifiy(source, knockback);

            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            if (source.FlipDirection)
                direction.x *= -1;

            Facing = direction.x > 0;

            Rigidbody.AddForce(direction * knockback);
        }

        public void Heal(IHealer source) {
            if (source == null || Damageable == null)
                return;

            float healing = Mathf.Abs(source.BaseHealing);

            if (Healing.Count > 0)
                healing = Healing.Modifiy(source, healing);

            _damageable.Heal(source, healing);

            if (OnHeal != null)
                OnHeal(source, healing);
        }
        #endregion

        #region Internal Methods
        internal float ModifyDamage(float baseDamage) {
            if (DamageDealt.Count <= 0)
                return baseDamage;
            return DamageDealt.Modifiy(baseDamage);
        }

        internal void AddCharacterComponent(CharacterComponent component) {
            if (component == null)
                return;
            components.Add(component);
        }

        internal void RemoveCharacterComponent(CharacterComponent component) {
            if (component == null)
                return;
            components.Remove(component);
        }
        #endregion

        #region Unity Callbacks
        protected virtual void Awake() {
            gameObject.tag = SmashGame.Config.PlayerTag;

            components = new HashSet<CharacterComponent>();
            ground = new HashSet<Collider>();

            DamageTaken = new ModifierList<IDamager>();
            KnockbackTaken = new ModifierList<IKnockbacker>();
            Healing = new ModifierList<IHealer>();

            DamageDealt = new ModifierList();
            KnockbackDealt = new ModifierList();

            MovementCollider = GetComponent<CapsuleCollider>();
            MovementCollider.isTrigger = false;

            gameObject.layer = PlayerLayer;

            Rigidbody = GetComponent<Rigidbody>();
            Rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

            Animator = GetComponent<Animator>();

            AttachRequiredComponents();

            CharacterAnimationBehaviour[] animationBehaviors = Animator.GetBehaviours<CharacterAnimationBehaviour>();
            foreach (CharacterAnimationBehaviour stateBehaviour in animationBehaviors)
                stateBehaviour.SetCharacter(this);
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
            Rigidbody.AddForce(-Vector3.up*_gravity);

            if (InputSource == null)
                return;
            
            Vector2 movement = InputSource.Movement;
            
            Animator.SetFloat(_animHInput, Mathf.Abs(movement.x));
            Animator.SetFloat(_animVInput, movement.y);

            //Ensure that the character is walking in the right direction
            if ((movement.x > 0 && Facing) ||
                (movement.x < 0 && !Facing))
                Facing = !Facing;

            if (InputSource.Jump)
                Jump();
            else if (InputSource.Attack)
                Attack();
            else if (InputSource.Special)
                Special();
        }

        protected virtual void OnAnimatorMove() {
            //TODO: Merge Physics and Animation Movements here

            //_rigidbody.velocity = _animator.deltaPosition / Time.deltaTime;
        }

        protected virtual void OnCollisionEnter(Collision col) {
            ContactPoint[] points = col.contacts;
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