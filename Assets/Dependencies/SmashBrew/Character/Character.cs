using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
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
    public class Character : HouraiBehaviour {

        private static readonly Type[] RequiredComponents; 

        static Character() {
            var componentType = typeof (Component);
            var requiredComponentType = typeof (RequiredCharacterComponentAttribute);
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
        
        public Player Player { get; internal set; }
        public ICharacterInput InputSource { get; set; }

        public bool IsGrounded {
            get { return _isGrounded; }
            set {
                if (IsGrounded == value)
                    return;
                _isGrounded = value;
                Animator.SetBool(_grounded, value);
                if(OnGrounded != null)
                    OnGrounded();
            }
        }

        public bool IsDashing {
            get { return IsGrounded && _isDashing; }
            set { _isDashing = value; }
        }

        public void AddForce(Vector3 force) {
            Rigidbody.AddForce(force);
        }

        public void AddForce(float x, float y) {
            Rigidbody.AddForce(x, y, 0f);
        }

        public void AddForce(float x, float y, float z) {
            Rigidbody.AddForce(x, y, z);
        }

        public event Action OnGrounded;
        public event Action OnBlastZoneExit;

        internal void BlastZoneExit() {
            if(OnBlastZoneExit != null)
                OnBlastZoneExit();
        }

        void AttachRequiredComponents() {
            foreach (Type requriedType in RequiredComponents)
                if(gameObject.GetComponent(requriedType) == null)
                    gameObject.AddComponent(requriedType);
        }

        public T ApplyStatus<T>(float duration = -1f) where T : Status {
            T instance = GetComponentInChildren<T>() ?? gameObject.AddComponent<T>();
            instance.StartStatus(duration);
            return instance;
        }

        #region Animator Variables
        [SerializeField]
        private int _grounded;

        [SerializeField]
        private int _horizontalInput;

        [SerializeField]
        private int _verticalInput;
        #endregion

        [SerializeField]
        private float triggerSizeRatio = 1.5f;

        [SerializeField, HideInInspector]
        public string InternalName;

        #region Required Components

        private CapsuleCollider _triggerCollider;
        private CharacterDamageable _damageable;
        
        public CapsuleCollider MovementCollider { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public Animator Animator { get; private set; }

        public CharacterDamageable Damage {
            get {
                if (_damageable == null)
                    _damageable = GetComponentInChildren<CharacterDamageable>();
                return _damageable;
            }
            private set { _damageable = value; }
        }

        #endregion

        #region State Variables

        private bool _isGrounded;
        private bool _facing;
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

        #region Unity Callbacks
        protected virtual void Awake() {
            MovementCollider = GetComponent<CapsuleCollider>();
            MovementCollider.isTrigger = false;

            _triggerCollider = gameObject.AddComponent<CapsuleCollider>();
            _triggerCollider.isTrigger = true;

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
        }

        protected virtual void OnDisable() {
            // TODO: Find a better place to put this
            CameraController.RemoveTarget(this);
        }

        public void Reset() {
            AttachRequiredComponents();
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        protected virtual void Update() {
            if (InputSource == null)
                return;

            Vector2 movement = InputSource.Movement;
            Animator.SetFloat(_horizontalInput, Mathf.Abs(movement.x));
            Animator.SetFloat(_verticalInput, movement.y);
        }

        /// <summary>
        /// Called every physics update.
        /// </summary>
        protected virtual void FixedUpdate() {
            // Sync Trigger and Movement Colliders
            _triggerCollider.center = MovementCollider.center;
            _triggerCollider.direction = MovementCollider.direction;
            _triggerCollider.height = MovementCollider.height*triggerSizeRatio;
            _triggerCollider.radius = MovementCollider.radius*triggerSizeRatio;
        }

        protected virtual void OnAnimatorMove() {
            //TODO: Merge Physics and Animation Movements here

            //_rigidbody.velocity = _animator.deltaPosition / Time.deltaTime;
        }
        #endregion
    }

}