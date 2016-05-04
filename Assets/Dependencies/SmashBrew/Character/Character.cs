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
    [RequireComponent(typeof (Rigidbody), typeof (CapsuleCollider))]
    public sealed partial class Character : HouraiBehaviour, IDamageable, IHealable, IKnockbackable {
        private enum FacingMode {
            Rotation,
            Scale
        }

        #region Public Properties

        public Mediator CharacterEvents { get; private set; }

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
            get {
				if (_ground.Count > 0 && Rigidbody.velocity.y <= 0.05f) {
					foreach (var ground in _ground) {
						if (ground.gameObject.activeInHierarchy) {
							return true;
						}
					}
				}
				return false;
			}
        }

        /// <summary>
        /// Gets or sets whether the Character is currently fast falling or not
        /// </summary>
        public bool IsFastFalling { get; set; }

        public float FallSpeed {
            get { return IsFastFalling ? _fastFallSpeed : _maxFallSpeed; }
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
            get { return _jumpHeights == null ? 0 : _jumpHeights.Length; }
        }

        /// <summary>
        /// Can the Character currently jump?
        /// </summary>
        public bool CanJump {
            get { return JumpCount < MaxJumpCount; }
        }

		public void ResetCharacter() {
			_ground.Clear();
			_collided = false;
		}

        #endregion

        #region Runtime Variables

        private Transform[] _bones;
        private HashSet<Collider> _ground;
        private bool _facing;
        private float _lastTap;
		private bool _collided;

        private bool _jumpQueued;

        #endregion

        #region Serialized Variables

        [SerializeField]
        private GameObject _rootBone;

        [SerializeField]
        private FacingMode _facingMode;

        [Header("Physics")]
        [SerializeField, Tooltip("The acceleration downward per second applied")]
        private float _gravity = 9.86f;

        [SerializeField]
        private float _maxFallSpeed = 5f;

        [SerializeField, Tooltip("The fast falling speed applied")]
        private float _fastFallSpeed = 9f;

        [SerializeField, Tooltip("The heights of each jump")]
        private float[] _jumpHeights = {1.5f, 1.5f};

        [SerializeField]
        private Renderer[] _weapons;

        [SerializeField]
        private ParticleSystem[] _particles;

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

        public bool Tap(Vector2 direction) {
            if (direction.sqrMagnitude > 0) {
                _lastTap = Time.realtimeSinceStartup;
                return true;
            }
            return false;
        }

        /// <summary>
        /// How fast the character is currently walking/running.
        /// </summary>
        public float movementSpeed = 0;
        public float dashingSpeed = 3.2f;
        public float runningSpeed = 5.4f;
        public bool isDashing = false;
        public float maxMovementSpeed
        {
            get
            {
                return isDashing ? runningSpeed : dashingSpeed;
            }
        }
        public float acceleration = 5.2f;

        public Vector2 controlStick;

        public void Move(float? spd) {
            
			//Check to see if we can move or not. Fixes getting stuck on wall
			if (_collided && !IsGrounded) { //We are hitting a wall or someone.
				Ray ray = new Ray(transform.position,  Direction? -Vector3.right:Vector3.right);
				if(Physics.Raycast (ray, MovementCollider.radius * 2, 9))
				{
					return;//Raycast will ignore characters...probably.
				}

			}
            float speed = 0;
            if (spd.HasValue)
            {
                speed = spd.Value;
            }
            else
            {
                float hspeed = movementSpeed;
                float dir = hspeed >= 0 ? 1 : -1;
                hspeed = Math.Abs(hspeed);
                if (dir>0 == controlStick.x>0)
                {
                    //accelerate
                    hspeed += acceleration * FixedDeltaTime;
                }
                else if (dir > 0 != controlStick.x > 0)
                {
                    //brake
                    hspeed -= (acceleration*3.5f) * FixedDeltaTime;
                }
                hspeed = Math.Min(hspeed, maxMovementSpeed * Math.Abs(controlStick.x));
                speed = hspeed * dir;
            }
            Vector3 vel = Rigidbody.velocity;
            vel.x = speed;
            if (speed >= 0 != movementSpeed >= 0)
            {
                //Animator.SetBool(CharacterAnim.TurnAround, true);
            }
            else if (Math.Abs(speed) < Math.Abs(movementSpeed))
            {
                //Animator.SetBool(CharacterAnim.Braking, true);
            }
            Rigidbody.velocity = vel;
            movementSpeed = speed;
            if (speed>=0 != !Direction)
            {
                Direction = !(speed >= 0);
            }
        }

        public void Jump() {
            if (CanJump)
                _jumpQueued = true;
        }

        /// <summary>
        /// Actually applies the force to jump.
        /// </summary>
        void JumpImpl() {
            // Apply upward force to jump
			Vector3 force = Vector3.up * Mathf.Sqrt(2 * Gravity * _jumpHeights[JumpCount]);
			force.y -= Rigidbody.velocity.y;
			Rigidbody.AddForce(force, ForceMode.VelocityChange);

            JumpCount++;

            CharacterEvents.Publish(new PlayerJumpEvent { Ground = IsGrounded, RemainingJumps = MaxJumpCount - JumpCount });
        }

        public void SetWeaponVisibilty(int weapon, bool state) {
            if (_weapons[weapon])
                _weapons[weapon].enabled = state;
        }

        public void SetParticleVisibilty(int particle, bool state) {
            if(state)
                _particles[particle].Play();
            else
                _particles[particle].Stop();
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

            foreach (ParticleSystem particle in _particles)
               if(particle) 
                    particle.Stop();

            foreach(var weapon in _weapons)
                if (weapon)
                    weapon.enabled = false;

            // Initialize all animation behaviours
            BaseAnimationBehaviour.InitializeAll(Animator);
        }

        void AnimationUpdate() {
            Animator.SetBool(CharacterAnim.Grounded, IsGrounded);
            Animator.SetBool(CharacterAnim.Jump, _jumpQueued);
            Animator.SetBool(CharacterAnim.Tap, Time.realtimeSinceStartup - _lastTap > Config.Player.TapPersistence);

            _jumpQueued = false;
        }

       void FixedUpdate() {
            float grav = _gravity;
            movementSpeed *= 1 - FixedDeltaTime;
            if (IsGrounded)
            {
                //Simulates ground friction.
                grav += (grav*0.5f);
                movementSpeed *= 1 - (FixedDeltaTime*0.5f);
            }
            Rigidbody.AddForce(-Vector3.up * grav, ForceMode.Acceleration);

            Vector3 velocity = Rigidbody.velocity;

            //if (!IsFastFalling && InputSource != null && InputSource.Movement.y < 0)
            //    _fastFall = true;

            if (IsFastFalling || velocity.y < -FallSpeed)
                velocity.y = -FallSpeed;
			if (IsGrounded && Rigidbody.velocity.y <= 0f) {
               IsFastFalling = false;
               JumpCount = 0;
           }
            Rigidbody.velocity = velocity;
            gameObject.layer = (velocity.magnitude > Config.Physics.TangibleSpeedCap)
                ? Layers.Intangible
                : Layers.Character;


            AnimationUpdate();
        }

        void OnCollisionEnter(Collision col) {
            GroundCheck(col);
			_collided = true;
        }

        void GroundCheck(Collision collison) {
            ContactPoint[] points = collison.contacts;
            if (points.Length <= 0)
                return;

			float r2 = MovementCollider.radius * MovementCollider.radius;
            Vector3 bottom = transform.TransformPoint(MovementCollider.center - Vector3.up * MovementCollider.height / 2);
            foreach (ContactPoint contact in points)
				if ((contact.point - bottom).sqrMagnitude < r2)
                    _ground.Add(contact.otherCollider);
        }

        void OnCollisionStay(Collision col) {
            GroundCheck(col);
			_collided = true;
        }

        void OnCollisionExit(Collision col) {
            _ground.Remove(col.collider);
			_collided = false;
        }

        void Reset() {
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

            foreach (Type component in GetRequiredComponents())
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
            var componentType = typeof (Component);
            var requiredComponentType = typeof (RequiredCharacterComponentAttribute);
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

        void OnAnimatorMove() {
            //TODO: Merge Physics and Animation Movements here

            //_rigidbody.velocity = _animator.deltaPosition / Time.deltaTime;
        }

        #endregion
    }
}
