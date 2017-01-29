using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.SmashBrew {

    /// <summary> General character class for handling the physics and animations of individual characters </summary>
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public class Character : NetworkBehaviour, IHitboxController {

        [SyncVar(hook = "ChangeActive")]
        bool _isActive;

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

        Dictionary<int, Hitbox> _hitboxMap;

        public CharacterController Controller { get; private set; }

        public override void OnStartServer() { _isActive = true; }

        void ChangeActive(bool active) {
            _isActive = active;
            gameObject.SetActive(active);
        }

        void OnEnable() { _isActive = true; }
        void OnDisable() { _isActive = false; }

        /// <summary> Unity callback. Called on object instantiation. </summary>
        void Awake() {
            Events = new Mediator();
            Reset();
            Controller = GetComponent<CharacterController>();
        }

        void Reset() {
            gameObject.tag = Config.Tags.PlayerTag;
            gameObject.layer = Config.Tags.CharacterLayer;
        }
    }

}
