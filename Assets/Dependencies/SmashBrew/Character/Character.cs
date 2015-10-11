﻿using System;
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
    public partial class Character : HouraiBehaviour {

        private static readonly Type[] RequiredComponents;

        private static readonly int _animGrounded = Animator.StringToHash("grounded");
        private static readonly int _animHInput = Animator.StringToHash("horizontal input");
        private static readonly int _animVInput = Animator.StringToHash("vertical input");
        private static readonly int _animAttack = Animator.StringToHash("attack");
        private static readonly int _animSpecial = Animator.StringToHash("special");
        private static readonly int _animJump = Animator.StringToHash("jump");

        private HashSet<CharacterComponent> components;

        private bool _facing;

        [SerializeField]
        private FacingMode _facingMode;

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

        private enum FacingMode {

            Rotation,
            Scale

        }

        [SerializeField]
        private float _gravity = 9.86f;

        public float Gravity {
            get { return _gravity; }
            set { _gravity = Mathf.Abs(value); }
        }

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
                Animator.SetBool(_animGrounded, value);
                if(OnGrounded != null)
                    OnGrounded();
            }
        }

        [SerializeField]
        private float[] _jumpHeights = { 1.5f, 1.5f };

        public int JumpCount { get; private set; }

        public int MaxJumpCount {
            get { return _jumpHeights == null ? 0 : _jumpHeights.Length; }
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
        public event Action OnAttack;
        public event Action OnSpecial;
        public event Action OnJump;

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

        #region Required Components
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
            if (JumpCount >= _jumpHeights.Length)//Restricted)
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

        #region Unity Callbacks
        protected virtual void Awake() {
            components = new HashSet<CharacterComponent>();

            MovementCollider = GetComponent<CapsuleCollider>();
            MovementCollider.isTrigger = false;

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
        /// Called every frame.
        /// </summary>
        protected virtual void Update() {
            if (InputSource == null)
                return;

            Vector2 movement = InputSource.Movement;
            Animator.SetFloat(_animHInput, Mathf.Abs(movement.x));
            Animator.SetFloat(_animVInput, movement.y);
        }

        /// <summary>
        /// Called every physics update.
        /// </summary>
        protected virtual void FixedUpdate() {
            AddForce(-Vector3.up*_gravity);

            if (InputSource == null)
                return;
            
            Vector2 movement = InputSource.Movement;

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
        #endregion
    }

}