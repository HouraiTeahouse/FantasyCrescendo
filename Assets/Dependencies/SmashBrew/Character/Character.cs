using System;
using UnityEngine;
using System.Linq;
using HouraiTeahouse.Events;
using UnityConstants;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.SmashBrew {

    public interface IResettable {
        void OnReset();
    }

    public abstract class AbstractCharacterComponent : HouraiBehaviour, IResettable {

        public Character Character { get; set; }

        public Mediator Events {
            get { return Character.Events; }
        }

        public virtual void OnReset() {
        }
    }

    /// <summary>
    /// General character class for handling the physics and animations of individual characters
    /// </summary>
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [DisallowMultipleComponent]
    [RequireComponent(typeof (Rigidbody), typeof (CapsuleCollider))]
    [RequireComponent(typeof(PlayerDamage), typeof(PlayerKnockback))]
    [RequireComponent(typeof (Gravity), typeof (Ground))]
    public sealed class Character : HouraiBehaviour {
        private enum FacingMode {
            Rotation,
            Scale
        }

        #region Public Properties

        public Mediator Events { get; private set; }

        /// <summary>
        /// Gets how many bones the Character has.
        /// </summary>
        public int BoneCount {
            get { return _bones.Length; }
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
            foreach(IResettable resetable in _resetableComponents)
                if(resetable != null)
                    resetable.OnReset();
		}

        #endregion

        #region Runtime Variables

        private Transform[] _bones;
        private bool _facing;
        private float _lastTap;
        private bool _jumpQueued;

        #endregion

        #region Serialized Variables

        [SerializeField]
        private GameObject _rootBone;

        [SerializeField]
        private FacingMode _facingMode;

        [Header("Physics")]
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
        public Gravity Gravity { get; private set; }
        public Ground Ground { get; private set; }
        public PlayerDamage Damage { get; private set; }

        private IResettable[] _resetableComponents;

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

        public void Move(float speed) {
			//Check to see if we can move or not. Fixes getting stuck on wall
			//if (_collided && !Ground) { //We are hitting a wall or someone.
			//	if(Physics.Raycast(transform.position, Direction ? -Vector3.right : Vector3.right, MovementCollider.radius * 2, 9))
			//		return; //Raycast will ignore characters...probably.
			//}
			Vector3 vel = Rigidbody.velocity;
            vel.x = speed;

            if (Direction)
                vel.x *= -1;
            Rigidbody.velocity = vel;
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

            Events.Publish(new PlayerJumpEvent { Ground = Ground, RemainingJumps = MaxJumpCount - JumpCount });
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

        void GetCharacterComponents() {
            Ground = GetComponent<Ground>();
            Gravity = GetComponent<Gravity>();
            Damage = GetComponent<PlayerDamage>();

            _resetableComponents = GetComponentsInChildren<IResettable>();
        }

        /// <summary>
        /// Unity callback. Called on object instantiation.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            Events = new Mediator();
            GetCharacterComponents();
            Reset();

            GameObject root = gameObject;
            if (_rootBone)
                root = _rootBone;
            _bones = root.GetComponentsInChildren<Transform>();

            MovementCollider = GetComponent<CapsuleCollider>();

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
            Animator.SetBool(CharacterAnim.Grounded, Ground);
            Animator.SetBool(CharacterAnim.Jump, _jumpQueued);
            Animator.SetBool(CharacterAnim.Tap, Time.realtimeSinceStartup - _lastTap > Config.Player.TapPersistence);

            _jumpQueued = false;
        }

       void FixedUpdate() {
            float grav = Gravity;
            //Simulates ground friction.
            if (Ground)
                grav *= 2.5f;
            Rigidbody.AddForce(-Vector3.up * grav, ForceMode.Acceleration);

            Vector3 velocity = Rigidbody.velocity;

            //if (!IsFastFalling && InputSource != null && InputSource.Movement.y < 0)
            //    _fastFall = true;

            if (IsFastFalling || velocity.y < -FallSpeed)
                velocity.y = -FallSpeed;
			if (Ground && Rigidbody.velocity.y <= 0f) {
               IsFastFalling = false;
               JumpCount = 0;
           }
            Rigidbody.velocity = velocity;
            gameObject.layer = (velocity.magnitude > Config.Physics.TangibleSpeedCap)
                ? Layers.Intangible
                : Layers.Character;

            AnimationUpdate();
        }

        void Reset() {
            MovementCollider = GetComponent<CapsuleCollider>();
            MovementCollider.isTrigger = false;

            gameObject.tag = Tags.Player;
            gameObject.layer = Layers.Character;

            Rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
            Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            Rigidbody.isKinematic = false;
            Rigidbody.useGravity = false;

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
