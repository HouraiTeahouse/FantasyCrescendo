using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;
#if UNITY_EDITOR
using System.Linq;
#endif

namespace Crescendo.API {

    /// <summary>
    /// General character class for handling the physics and animations of individual characters
    /// </summary>
    /// Author: James Liu
    /// Authored on 07/01/2015
    [DisallowMultipleComponent]
    [DefineCategories("Animation")]
    [RequireComponent(typeof (Animator), typeof (Rigidbody), typeof (CapsuleCollider))]
    public class Character : GensoBehaviour {
        
        private CapsuleCollider triggerCollider;

        [DontSerialize, Hide]
        public int PlayerNumber { get; internal set; }

        public Color PlayerColor {
            get { return Game.GetPlayerColor(PlayerNumber); }
        }

        [DontSerialize, Hide]
        public ICharacterInput InputSource { get; set; }

        public bool IsIsGrounded {
            get { return _isGrounded; }
            set {
                if (IsIsGrounded == value)
                    return;
                _isGrounded = value;
                Animator.SetBool(_grounded, value);
                OnGrounded.SafeInvoke();
            }
        }

        public bool IsDashing {
            get { return IsIsGrounded && _isDashing; }
            set { _isDashing = value; }
        }

        public bool IsFastFalling {
            get { return !IsIsGrounded && InputSource != null && InputSource.Crouch; }
        }

        public bool IsCrouching {
            get { return IsIsGrounded && InputSource != null && InputSource.Crouch; }
        }

        public float Height {
            get { return MovementCollider.height; }
        }

        public void Knockback(float baseKnockback) {
            OnKnockback.SafeInvoke();
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
        public event Action OnKnockback;

        internal void BlastZoneExit() {
            OnBlastZoneExit.SafeInvoke();
        }

        void AttachRequiredComponents() {
            IEnumerable<Type> componentTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                                from assemblyType in domainAssembly.GetTypes()
                                                where
                                                    assemblyType.IsA<Component>() &&
                                                    !assemblyType.IsAbstract &&
                                                    assemblyType.IsDefined(
                                                                           typeof (RequiredCharacterComponentAttribute),
                                                                           true)
                                                select assemblyType);

            foreach (Type requriedType in componentTypes) {
                if (requriedType != null)
                    this.GetOrAddComponent(requriedType);
            }
        }

        public T AddStatus<T>(float duration) where T : Status {
            T status = gameObject.AddComponent<T>();
            status.StartStatus(duration);
            return status;
        }

        public void RemoveStatus<T>() where T : Status {
            T status = GetComponent<T>();
            if(status != null)
                Destroy(status);
        }

        #region Animator Variables
        [Serialize]
        [Category("Animation")]
        [AnimVar(Filter = ParameterType.Bool)]
        private int _grounded;

        [Serialize]
        [Category("Animation")]
        [AnimVar(Filter = ParameterType.Float)]
        private int _horizontalInput;

        [Serialize]
        [Category("Animation")]
        [AnimVar(Filter = ParameterType.Float)]
        private int _verticalInput;
        #endregion

        [Serialize]
        private float triggerSizeRatio = 1.5f;

        public string InternalName {
            get;
            set;
        }

        #region Required Components

        [DontSerialize, Hide]
        public CapsuleCollider MovementCollider { get; private set; }

        [DontSerialize, Hide]
        public Rigidbody Rigidbody { get; private set; }

        [DontSerialize, Hide]
        public Animator Animator { get; private set; }

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

            triggerCollider = gameObject.AddComponent<CapsuleCollider>();
            triggerCollider.isTrigger = true;

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

        protected virtual void Reset() {
            AttachRequiredComponents();
        }

        protected virtual void Update() {
            // Sync Trigger and Movement Colliders
            triggerCollider.center = MovementCollider.center;
            triggerCollider.direction = MovementCollider.direction;
            triggerCollider.height = MovementCollider.height*triggerSizeRatio;
            triggerCollider.radius = MovementCollider.radius*triggerSizeRatio;

            if (InputSource == null)
                return;

            Vector2 movement = InputSource.Movement;
            Animator.SetFloat(_horizontalInput, Mathf.Abs(movement.x));
            Animator.SetFloat(_verticalInput, movement.y);
        }

        protected virtual void OnAnimatorMove() {
            //TODO: Merge Physics and Animation Movements here

            //_rigidbody.velocity = _animator.deltaPosition / Time.deltaTime;
        }
        #endregion
    }

}