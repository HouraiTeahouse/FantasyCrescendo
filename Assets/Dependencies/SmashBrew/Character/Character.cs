using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using HouraiTeahouse.Events;
using HouraiTeahouse.SmashBrew.Util;
using UnityConstants;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.SmashBrew {

    /// <summary>
    /// General character class for handling the physics and animations of individual characters
    /// </summary>
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public partial class Character : HouraiBehaviour, IDamageable, IHealable, IKnockbackable {

        private enum FacingMode {
            Rotation,
            Scale
        }

        #region Public Properties

        public Mediator CharacterEvents {
            get; private set;
        }

        /// <summary>
        /// Gets how many bones the Character has.
        /// </summary>
        public int BoneCount {
            get { return _bones.Length; }
        }

        /// <summary>
        /// Gets whether the Character is currently on solid Ground.
        /// Assumed to be in the air when false.
        /// </summary>
        public bool IsGrounded {
            get { return Animator.GetBool(CharacterAnimVars.Grounded); }
            private set {
                if (IsGrounded == value)
                    return;
                if (value) {
                    IsFastFalling = false;
                    JumpCount = 0;
                }
                Animator.SetBool(CharacterAnimVars.Grounded, value);
                CharacterEvents.Publish(new PlayerGroundEvent { Grounded = value });
            }
        }

        /// <summary>
        /// Gets or sets whether the Character is currently fast falling or not
        /// </summary>
        public bool IsFastFalling { get; set; }

        public float FallSpeed {
            get {
                return IsFastFalling ? _fastFallSpeed : _maxFallSpeed;
            }
        }

        /// <summary>
        /// The direction the character is currently facing.
        /// If set to true, the character faces the right.
        /// If set to false, the character faces the left.
        /// 
        /// The method in which the character is flipped depends on what the Facing Mode parameter is set to.
        /// </summary>
        public bool Direction {
            get {
                if (_facingMode == FacingMode.Rotation)
                    return transform.eulerAngles.y > 179f;
                return transform.localScale.x > 0;
            }
            set {
                if (_facing == value)
                    return;

                _facing = value;
                if (_facingMode == FacingMode.Rotation)
                    transform.Rotate(0f, 180f, 0f);
                else
                    transform.localScale *= -1;
            }
        }

        /// <summary>
        /// Gets or sets the magnitude of gravity applied to the Character.
        /// </summary>
        public float Gravity {
            get { return _gravity; }
            set { _gravity = Mathf.Abs(value); }
        }

        /// <summary>
        /// Gets how many remaining jumps the Character currently has.
        /// </summary>
        public int JumpCount { get; private set; }

        /// <summary>
        /// Gets the maximum number of jumps the Character can preform.
        /// </summary>
        public int MaxJumpCount {
            get { return _jumpPower == null ? 0 : _jumpPower.Length; }
        }
        #endregion

        #region Runtime Variables
        private Transform[] _bones;
        private HashSet<Collider> _ground;
        private bool _facing;
        #endregion

        #region Serialized Variables
        [SerializeField]
        private GameObject _rootBone;

        [SerializeField]
        private FacingMode _facingMode;

        [Header("Physics")]
        [SerializeField]
        private float _gravity = 9.86f;

        [SerializeField]
        private float _fastFallSpeed = 9f;

        [SerializeField]
        private float _maxFallSpeed = 5f;

        [SerializeField]
        private float[] _jumpPower = {1.5f, 1.5f};
        #endregion

        #region Public Events
        public event Action<Attack.Type, Attack.Direction, int> OnAttack;
        #endregion

        #region Required Components
        public CapsuleCollider MovementCollider { get; private set; }
        #endregion

        #region Public Action Methods
        public Transform GetBone(int boneIndex) {
            if (boneIndex < 0 || boneIndex >= BoneCount)
                return transform;
            return _bones[boneIndex];
        }

        public void Move(float speed) {
            Vector3 vel = Rigidbody.velocity;
            vel.x = speed;

            if (Direction)
                vel.x *= -1;

            Rigidbody.velocity = vel;
        }

        public void Jump() {
            if (JumpCount >= MaxJumpCount)//Restricted)
                return;

            // Apply upward force to jump
            Rigidbody.velocity += Vector3.up * Mathf.Sqrt(2 * Gravity * _jumpPower[JumpCount]);

            JumpCount++;

            CharacterEvents.Publish(new PlayerJumpEvent { Ground = IsGrounded, RemainingJumps = MaxJumpCount - JumpCount });
        }
        #endregion

        #region Internal Methods
        internal void Attack(Attack.Type type, Attack.Direction direction, int index) {
            if (OnAttack != null)
                OnAttack(type, direction, index);
        }
        #endregion

        #region Unity Callbacks

        /// <summary>
        /// Unity callback. Called on object instantiation.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            CharacterEvents = new Mediator();
            Reset();

            GameObject root = gameObject;
            if (_rootBone)
                root = _rootBone;
            _bones = root.GetComponentsInChildren<Transform>();

            MovementCollider = GetComponent<CapsuleCollider>();
            _ground = new HashSet<Collider>();

            DamageModifiers = new ModifierGroup<object>();
            HealingModifiers = new ModifierGroup<object>();
            KnockbackModifiers = new ModifierGroup<Vector2>();

            // Initialize all animation behaviours
            BaseAnimationBehaviour.InitializeAll(Animator);
        }

        protected virtual void FixedUpdate() {
            Rigidbody.AddForce(-Vector3.up * _gravity);

            Vector3 velocity = Rigidbody.velocity;

            //if (!IsFastFalling && InputSource != null && InputSource.Movement.y < 0)
            //    _fastFall = true;

            if (IsFastFalling || velocity.y < -FallSpeed)
                velocity.y = -FallSpeed;

            Rigidbody.velocity = velocity;
            gameObject.layer = (velocity.magnitude > Config.Instance.TangibleSpeedCap) ? Layers.Intangible : Layers.Character;
        }

        protected virtual void OnCollisionEnter(Collision col) {
            ContactPoint[] points = col.contacts;
            if (points.Length <= 0)
                return;

            float r2 = MovementCollider.radius * MovementCollider.radius;
            Vector3 bottom = transform.TransformPoint(MovementCollider.center - Vector3.up * MovementCollider.height / 2);
            foreach (ContactPoint contact in points)
                if ((contact.point - bottom).sqrMagnitude < r2)
                    _ground.Add(contact.otherCollider);
            IsGrounded = _ground.Count > 0;
        }

        protected virtual void OnCollisionExit(Collision col) {
            if (_ground.Remove(col.collider))
                IsGrounded = _ground.Count > 0;
        }

        protected virtual void Reset() {
            MovementCollider = GetComponent<CapsuleCollider>();
            MovementCollider.isTrigger = false;

            gameObject.tag = Tags.Player;
            gameObject.layer = Layers.Character;

            Rigidbody rb = Rigidbody;
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.isKinematic = false;
            rb.useGravity = false;

            Animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
#if UNITY_EDITOR
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            foreach(Type component in GetRequiredComponents())
                if (!gameObject.GetComponent(component))
                    gameObject.AddComponent(component);
#endif
        }

#if UNITY_EDITOR

        /// <summary>
        /// Editor only function that gets all of the required component types a Character needs.
        /// </summary>
        /// <returns>an array of all of the concrete component types marked with RequiredCharacterComponent</returns>
        public static Type[] GetRequiredComponents() {
            var componentType = typeof(Component);
            var requiredComponentType = typeof(RequiredCharacterComponentAttribute);
            // Use reflection to find required Components for Characters and statuses
            // Enumerate all concrete Component types
            return (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                  from assemblyType in domainAssembly.GetTypes()
                                  where
                                      assemblyType != null &&
                                      !assemblyType.IsAbstract &&
                                      componentType.IsAssignableFrom(assemblyType) &&
                                      assemblyType.IsDefined(requiredComponentType, true)
                                  select assemblyType).ToArray();
        }
#endif

        protected virtual void OnAnimatorMove() {
            //TODO: Merge Physics and Animation Movements here

            //_rigidbody.velocity = _animator.deltaPosition / Time.deltaTime;
        }
        #endregion
    }

}
