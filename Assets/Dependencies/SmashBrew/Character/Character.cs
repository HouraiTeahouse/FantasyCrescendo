using HouraiTeahouse.SmashBrew.States;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.SmashBrew.Characters {

    /// <summary> General character class for handling the physics and animations of individual characters </summary>
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(MovementState))]
    public class Character : NetworkBehaviour, IHitboxController, IRegistrar<ICharacterComponent> {

        public CharacterController Controller { get; private set; }
        public MovementState Movement { get; private set; }
        public PhysicsState Physics { get; private set; }
        public StateController<CharacterState, CharacterStateContext> StateController { get; private set; }
        public CharacterStateContext Context { get; private set; }

        public CharacterControllerBuilder States {
            get { return _controller; }
        }

        Dictionary<int, Hitbox> _hitboxMap;
        List<ICharacterComponent> _components;

        [SerializeField]
        CharacterControllerBuilder _controller;

        /// <summary> Unity callback. Called on object instantiation. </summary>
        void Awake() {
            gameObject.tag = Config.Tags.PlayerTag;
            gameObject.layer = Config.Tags.CharacterLayer;
            if (_controller == null)
                throw new InvalidOperationException("Cannot start a character without a State Controller!");
            StateController = _controller.BuildCharacterControllerImpl(
                new StateControllerBuilder<CharacterState, CharacterStateContext>());
            StateController.OnStateChange += (b, a) => Log.Debug("{0} changed states: {1} => {2}".With(name, b.Name, a.Name));
            Context = new CharacterStateContext();
            _hitboxMap = new Dictionary<int, Hitbox>();
            _components = new List<ICharacterComponent>();
            Controller = this.SafeGetComponent<CharacterController>();
            Movement = this.SafeGetComponent<MovementState>();
            Physics = this.SafeGetComponent<PhysicsState>();
        }

        void IRegistrar<Hitbox>.Register(Hitbox hitbox) {
            int id = Argument.NotNull(hitbox).ID;
            if (_hitboxMap.ContainsKey(id))
                Log.Error("Hitboxes {0} and {1} on {2} have the same id. Ensure that they have different IDs.",
                    hitbox,
                    _hitboxMap[id],
                    gameObject.name);
            else
                _hitboxMap.Add(id, hitbox);
        }

        bool IRegistrar<Hitbox>.Unregister(Hitbox obj) {
            return _hitboxMap.Remove(Argument.NotNull(obj).ID);
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
                if (hitbox.ResetType())
                    Log.Info("{0} {1}", this, hitbox);
            }
        }

        #region Unity Callbacks
        void OnEnable() { _isActive = true; }
        void OnDisable() { _isActive = false; }
        public override void OnStartServer() { _isActive = true; }
        void LateUpdate() {
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
            foreach (IResettable resetable in GetComponentsInChildren<IResettable>().IgnoreNulls())
                resetable.OnReset();
        }
        #endregion

#pragma warning disable 414
        [SerializeField, ReadOnly, SyncVar(hook = "ChangeActive")]
        bool _isActive;
#pragma warning restore 414


        void ChangeActive(bool active) {
            _isActive = active;
            gameObject.SetActive(active);
        }

    }

}
