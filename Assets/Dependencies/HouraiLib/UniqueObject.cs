using System;
using UnityEngine;
using System.Collections.Generic;

namespace HouraiTeahouse {

    /// <summary>
    /// Component that marks a unique object.
    /// Objects instantiated with this attached only allows one to exist.
    /// Trying to create/instantiate more copies will have the object destroyed instantly.
    /// </summary>
    [DisallowMultipleComponent]
    public class UniqueObject : MonoBehaviour {

        private static Dictionary<string, UniqueObject> _allIds;

        [SerializeField, ReadOnly]
        private string _id;

        /// <summary>
        /// The unique ID of the object.
        /// </summary>
        public string ID {
            get {
                return _id;
            }
        }

        /// <summary>
        /// Unity Callback. Called on object instantiation.
        /// </summary>
        void Awake() {
            if (_allIds == null) {
                _allIds = new Dictionary<string, UniqueObject>();
            }
            if(_allIds.ContainsKey(ID)) {
                Destroy(gameObject);
                return;
            }
            _allIds[ID] = this;
        }

        /// <summary>
        /// Unity callback. Called on object destruction.
        /// </summary>
        void OnDestroy() {
            if (_allIds == null || _allIds[ID] != this)
                return;
            _allIds.Remove(ID);
            if (_allIds.Count <= 0) {
                _allIds = null;
            }
        }

        /// <summary>
        /// Unity callback. Called on editor reset.
        /// </summary>
        void Reset() {
            _id = Guid.NewGuid().ToString();
        }

    }
}
