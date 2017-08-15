using HouraiTeahouse.SmashBrew.States;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    /// <summary> General character class for handling the physics and animations of individual characters </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(MovementState))]
    public class Character : NetworkBehaviour, IRegistrar<ICharacterComponent> {

        public CharacterController Controller { get; private set; }
        public MovementState Movement { get; private set; }
        public StateController<CharacterState, CharacterStateContext> StateController { get; private set; }
        public CharacterStateContext Context { get; private set; }

        public CharacterControllerBuilder States {
            get { return _controller; }
        }

        Dictionary<int, Hitbox> _hitboxMap;
        Dictionary<int, CharacterState> _stateMap;
        List<Hitbox> _hurtboxes;
        List<ICharacterComponent> _components;
        HashSet<object> _hitHistory;

        [SerializeField]
        CharacterControllerBuilder _controller;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            gameObject.tag = Config.Tags.PlayerTag;
            gameObject.layer = Config.Tags.CharacterLayer;
            if (_controller == null)
                throw new InvalidOperationException("Cannot start a character without a State Controller!");
            StateController = _controller.BuildCharacterControllerImpl(
                new StateControllerBuilder<CharacterState, CharacterStateContext>());
            if (Debug.isDebugBuild)
                StateController.OnStateChange += (b, a) => Log.Debug("{0} changed states: {1} => {2}".With(name, b.Name, a.Name));
            Context = new CharacterStateContext();
            _hitboxMap = new Dictionary<int, Hitbox>();
            _hurtboxes = new List<Hitbox>();
            _components = new List<ICharacterComponent>();
            _stateMap = StateController.States.ToDictionary(s => s.AnimatorHash);
            _hitHistory = new HashSet<object>();
            Controller = this.SafeGetComponent<CharacterController>();
            Movement = this.SafeGetComponent<MovementState>();
            EstablishImmunityChanges();
        }

        public bool CheckHistory(object obj) {
            var result = _hitHistory.Contains(obj);
            if (!result)
                _hitHistory.Add(obj);
            return result;
        }

        void EstablishImmunityChanges() {
            var typeMap = new Dictionary<ImmunityType, Hitbox.Type> {
                {ImmunityType.Normal, Hitbox.Type.Damageable},
                {ImmunityType.Intangible, Hitbox.Type.Intangible},
                {ImmunityType.Invincible, Hitbox.Type.Invincible}
            };
            StateController.OnStateChange += (b, a) => {
                if (_hitboxMap == null || _hitboxMap.Count < 0)
                    return;
                foreach (var hitbox in _hitboxMap.Values)
                    hitbox.ResetState();
                _hitHistory.Clear();
            };
            StateController.OnStateChange += (b, a) => {
                if (_hurtboxes == null || _hurtboxes.Count < 0)
                    return;
                var hitboxType = Hitbox.Type.Damageable;
                typeMap.TryGetValue(a.Data.DamageType, out hitboxType);
                foreach (var hurtbox in _hurtboxes)
                    hurtbox.CurrentType = hitboxType;
            };
        }

        void IRegistrar<ICharacterComponent>.Register(ICharacterComponent component) {
            if (_components.Contains(Argument.NotNull(component)))
                return;
            _components.Add(component);
        }

        bool IRegistrar<ICharacterComponent>.Unregister(ICharacterComponent component) {
            return _components.Remove(component);
        }

        /// <summary> Retrieves a hitbox given it's ID. </summary>
        /// <param name="id"> the ID to look for </param>
        /// <returns> the hitbox if found, null otherwise. </returns>
        public Hitbox GetHitbox(int id) {
            return _hitboxMap.GetOrDefault(id);
        }

        public void ResetAllHitboxes() {
            foreach (Hitbox hitbox in Hitboxes.IgnoreNulls()) {
                if (hitbox.ResetState())
                    Log.Info("{0} {1}", this, hitbox);
            }
        }

        #region Unity Callbacks

        void OnEnable() { _isActive = true; }

        void OnDisable() { _isActive = false; }

        void LateUpdate() {
            if (!isLocalPlayer || SmashTimeManager.Paused)
                return;
            foreach (var component in _components)
                component.UpdateStateContext(Context);
            StateController.UpdateState(Context);
        }
        #endregion

        #region Public Properties
        /// <summary> Gets an immutable collection of hitboxes that belong to </summary>
        public ICollection<Hitbox> Hitboxes {
            get { return _hitboxMap.Values; }
        }

        public void ResetCharacter() {
            StateController.ResetState();
            _hitHistory.Clear();
            foreach (IResettable resetable in GetComponentsInChildren<IResettable>().IgnoreNulls())
                resetable.OnReset();
        }
        #endregion

#pragma warning disable 414
        [SyncVar(hook = "ChangeActive")]
        bool _isActive;
#pragma warning restore 414

        // Network Callbacks

        void ChangeActive(bool active) {
            _isActive = active;
            gameObject.SetActive(active);
        }

    }

}
