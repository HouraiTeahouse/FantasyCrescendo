using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityConstants;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.SmashBrew {

    /// <summary> General character class for handling the physics and animations of individual characters </summary>
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    [RequireComponent(typeof(PlayerDamage), typeof(PlayerKnockback))]
    [RequireComponent(typeof(Gravity), typeof(Ground))]
    public class Character : BaseBehaviour, IHitboxController {

        public SmashCharacterController Controller { get; private set; }

        protected virtual void BuildController(SmashCharacterControllerBuildder builder) {
            BuildDefaultController(builder);
        }

        protected void BuildDefaultController(SmashCharacterControllerBuildder builder) {
            //TODO(james7312): Implement
            throw new NotImplementedException();
        }

        void IRegistrar<Hitbox>.Register(Hitbox hitbox) {
            Argument.NotNull(hitbox);
            if (_hitboxMap == null)
                _hitboxMap = new Dictionary<int, Hitbox>();
            int id = hitbox.ID;
            if (_hitboxMap.ContainsKey(hitbox.ID))
                Log.Error("Hitboxes {0} and {1} on {2} have the same id. Ensure that they have different IDs.",
                    hitbox,
                    _hitboxMap[id],
                    gameObject.name);
            else
                _hitboxMap.Add(id, hitbox);
        }

        bool IRegistrar<Hitbox>.Unregister(Hitbox obj) {
            return _hitboxMap != null && _hitboxMap.Remove(Argument.NotNull(obj).ID);
        }

        /// <summary> Retrieves a hitbox given it's ID. </summary>
        /// <param name="id"> the ID to look for </param>
        /// <returns> the hitbox if found, null otherwise. </returns>
        public Hitbox GetHitbox(int id) {
            return _hitboxMap.GetOrDefault(id);
        }

        public void ResetAllHitboxes() {
            foreach (Hitbox hitbox in Hitboxes.IgnoreNulls()) {
                if (hitbox.ResetType())
                    Log.Info("{0} {1}", this, hitbox);
            }
        }

        #region Public Properties

        public Mediator Events { get; private set; }

        /// <summary> Gets or sets whether the Character is currently fast falling or not </summary>
        public bool IsFastFalling { get; set; }

        public float FallSpeed {
            get { return IsFastFalling ? _fastFallSpeed : _maxFallSpeed; }
        }

        ///// <summary> The direction the character is currently facing. If set to true, the character faces the right. If set to
        ///// false, the character faces the left. The method in which the character is flipped depends on what the Facing Mode
        ///// parameter is set to. </summary>
        //public bool Direction {
        //    get {
        //        if (_facingMode == FacingMode.Rotation)
        //            return transform.eulerAngles.y > 179f;
        //        return transform.localScale.x > 0;
        //    }
        //    set {
        //        if (_facing == value)
        //            return;
        //        _facing = value;
        //        if (_facingMode == FacingMode.Rotation)
        //            transform.Rotate(0f, 180f, 0f);
        //        else
        //            transform.localScale *= -1;
        //    }
        //}

        /// <summary> Gets how many remaining jumps the Character currently has. </summary>
        public int JumpCount { get; private set; }

        /// <summary> Gets an immutable collection of hitboxes that belong to </summary>
        public ICollection<Hitbox> Hitboxes {
            get { return _hitboxMap.Values; }
        }

        /// <summary> Gets the maximum number of jumps the Character can preform. </summary>
        public int MaxJumpCount {
            get { return _jumpHeights == null ? 0 : _jumpHeights.Length; }
        }

        /// <summary> Can the Character currently jump? </summary>
        public bool CanJump {
            get { return JumpCount < MaxJumpCount; }
        }

        public void ResetCharacter() {
            foreach (IResettable resetable in GetComponentsInChildren<IResettable>().IgnoreNulls())
                resetable.OnReset();
        }

        #endregion

        #region Runtime Variables

        bool _facing;
        float _lastTap;
        bool _jumpQueued;
        Dictionary<int, Hitbox> _hitboxMap;

        #endregion

        #region Serialized Variables

        //[SerializeField]
        //FacingMode _facingMode;

        [Header("Physics")]
        [SerializeField]
        float _maxFallSpeed = 5f;

        [SerializeField]
        [Tooltip("The fast falling speed applied")]
        float _fastFallSpeed = 9f;

        [SerializeField]
        [Tooltip("The heights of each jump")]
        float[] _jumpHeights = {1.5f, 1.5f};

        [SerializeField]
        Renderer[] _weapons;

        [SerializeField]
        ParticleSystem[] _particles;

        #endregion

        #region Required Components

        public CapsuleCollider MovementCollider { get; private set; }
        public Gravity Gravity { get; private set; }
        public Ground Ground { get; private set; }
        public PlayerDamage Damage { get; private set; }

        #endregion

        #region Public Action Methods

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

            //if (Direction)
            //    vel.x *= -1;
            Rigidbody.velocity = vel;
        }

        public void Jump() {
            if (CanJump)
                _jumpQueued = true;
        }

        /// <summary> Actually applies the force to jump. </summary>
        internal void JumpImpl() {
            // Apply upward force to jump
            Vector3 force = Vector3.up * Mathf.Sqrt(2 * Gravity * _jumpHeights[JumpCount]);
            force.y -= Rigidbody.velocity.y;
            Rigidbody.AddForce(force, ForceMode.VelocityChange);

            JumpCount++;

            Events.Publish(new PlayerJumpEvent {Ground = Ground, RemainingJumps = MaxJumpCount - JumpCount});
        }

        public void SetWeaponVisibilty(int weapon, bool state) {
            if (_weapons[weapon])
                _weapons[weapon].enabled = state;
        }

        public void SetParticleVisibilty(int particle, bool state) {
            if (state)
                _particles[particle].Play();
            else
                _particles[particle].Stop();
        }

        #endregion

        #region Unity Callbacks

        void GetCharacterComponents() {
            Ground = GetComponent<Ground>();
            Gravity = GetComponent<Gravity>();
            Damage = GetComponent<PlayerDamage>();
        }

        /// <summary> Unity callback. Called on object instantiation. </summary>
        protected override void Awake() {
            base.Awake();
            Events = new Mediator();
            GetCharacterComponents();
            Reset();

            if (Animator != null)
                Animator.gameObject.GetOrAddComponent<CharacterAnimationEvents>();
            else
                Log.Error("Character {0} does not have an Animator component in its hiearchy", name);

            MovementCollider = GetComponent<CapsuleCollider>();

            foreach (ParticleSystem particle in _particles.IgnoreNulls())
                particle.Stop();

            foreach (Renderer weapon in _weapons.IgnoreNulls())
                weapon.enabled = false;

            StartCoroutine(InitializeAnimator());

            var builder = new SmashCharacterControllerBuildder();
            BuildDefaultController(builder);
            Controller = builder.Build();
        }

        IEnumerator InitializeAnimator() {
            yield return null;
            BaseAnimationBehaviour.InitializeAll(Animator);
        }

        void AnimationUpdate() {
            Animator.SetBool(CharacterAnim.Grounded, Ground);
            Animator.SetBool(CharacterAnim.Jump, _jumpQueued);
            Animator.SetBool(CharacterAnim.Tap, Time.realtimeSinceStartup - _lastTap > Config.Player.TapPersistence);

            _jumpQueued = false;
        }

        void FixedUpdate() {
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
            gameObject.layer = velocity.magnitude > Config.Physics.TangibleSpeedCap
                ? Layers.Intangible
                : Layers.Character;

            AnimationUpdate();
        }

        void Reset() {
            MovementCollider = GetComponent<CapsuleCollider>();
            MovementCollider.isTrigger = false;

            gameObject.tag = Tags.Player;
            gameObject.layer = Layers.Character;

            Rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            Rigidbody.isKinematic = false;
            Rigidbody.useGravity = false;

            Animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
#if UNITY_EDITOR
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            Type[] requiredComponents = GetRequiredComponents();
            foreach (Type component in requiredComponents) {
                if (!gameObject.GetComponent(component))
                    gameObject.AddComponent(component);
            }
#endif
        }

#if UNITY_EDITOR

        /// <summary> Editor only function that gets all of the required component types a Character needs. </summary>
        /// <returns> an array of all of the concrete component types marked with RequiredCharacterComponent </returns>
        public static Type[] GetRequiredComponents() {
            Type component = typeof(Component);
            // Use reflection to find required Components for Characters and statuses
            // Enumerate all concrete Component types
            return
                ReflectionUtilty.AllTypes.ConcreteClasses()
                    .IsAssignableFrom(component)
                    .WithAttribute<RequiredAttribute>()
                    .Keys()
                    .ToArray();
        }
#endif

        void OnAnimatorMove() {
            //TODO: Merge Physics and Animation Movements here
            //_rigidbody.velocity = _animator.deltaPosition / Time.deltaTime;
        }

        #endregion
    }

}
