using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace HouraiTeahouse {

    /// <summary> Component that marks a unique object. Objects instantiated with this attached only allows one to exist.
    /// Trying to create/instantiate more copies will have the object destroyed instantly. </summary>
    [DisallowMultipleComponent]
    public sealed class UniqueObject : MonoBehaviour, IUniqueEntity<int> {

        /// <summary> A collection of all of the UniqueObjects currently in the game. </summary>
        static Dictionary<int, UniqueObject> _allIds;

        [SerializeField]
        [ReadOnly]
        [Tooltip("The unique id for this object")]
        int _id;

        /// <summary> The unique ID of the object. </summary>
        public int ID {
            get { return _id; }
        }

        /// <summary> Unity Callback. Called on object instantiation. </summary>
        void Awake() {
            if (_allIds == null)
                _allIds = new Dictionary<int, UniqueObject>();
            if (_allIds.ContainsKey(ID)) {
                // Destroy only destroys the object after a frame is finished, which still allows
                // other code in other attached scripts to execute.
                // DestroyImmediate ensures that said code is not executed and immediately removes the
                // GameObject from the scene.
                Log.Info("[Unique Object] {0} (ID: {1}) already exists. Destroying {2}", _allIds[ID].name, ID, name);
                DestroyImmediate(gameObject);
                return;
            }
            _allIds[ID] = this;
        }

        /// <summary> Unity callback. Called on object destruction. </summary>
        void OnDestroy() {
            if (_allIds == null || _allIds[ID] != this)
                return;
            _allIds.Remove(ID);
            if (_allIds.Count <= 0)
                _allIds = null;
        }

        /// <summary> Unity callback. Called on editor reset. </summary>
        void Reset() { _id = new Random().Next(); }

    }

}
