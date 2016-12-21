using System;
using System.Collections;
using System.Collections.Generic;
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

        /// <summary> Gets an immutable collection of hitboxes that belong to </summary>
        public ICollection<Hitbox> Hitboxes {
            get { return _hitboxMap.Values; }
        }

        public void ResetCharacter() {
            foreach (IResettable resetable in GetComponentsInChildren<IResettable>().IgnoreNulls())
                resetable.OnReset();
        }

        #endregion

        #region Runtime Variables

        float _lastTap;
        bool _jumpQueued;
        Dictionary<int, Hitbox> _hitboxMap;

        #endregion

        #region Serialized Variables

        [SerializeField]
        Renderer[] _weapons;

        [SerializeField]
        ParticleSystem[] _particles;

        #endregion

        #region Required Components

        public CapsuleCollider MovementCollider { get; private set; }

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

        /// <summary> Unity callback. Called on object instantiation. </summary>
        protected override void Awake() {
            base.Awake();
            Events = new Mediator();
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
            Animator.SetBool(CharacterAnim.Jump, _jumpQueued);
            Animator.SetBool(CharacterAnim.Tap, Time.realtimeSinceStartup - _lastTap > Config.Player.TapPersistence);

            _jumpQueued = false;
        }

        void FixedUpdate() {
            Vector3 velocity = Rigidbody.velocity;

            //if (!IsFastFalling && InputSource != null && InputSource.Movement.y < 0)
            //    _fastFall = true;

            Rigidbody.velocity = velocity;
            gameObject.layer = velocity.magnitude > Config.Physics.TangibleSpeedCap
                ? Config.Tags.IntangibleLayer
                : Config.Tags.CharacterLayer;

            AnimationUpdate();
        }

        void Reset() {
            MovementCollider = GetComponent<CapsuleCollider>();
            MovementCollider.isTrigger = false;

            gameObject.tag = Config.Tags.PlayerTag;
            gameObject.layer = Config.Tags.CharacterLayer;

            Animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
        }

        void OnAnimatorMove() {
            //TODO: Merge Physics and Animation Movements here
            //_rigidbody.velocity = _animator.deltaPosition / Time.deltaTime;
        }

        #endregion
    }

}
