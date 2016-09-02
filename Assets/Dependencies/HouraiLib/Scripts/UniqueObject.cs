// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace HouraiTeahouse {

    /// <summary> Component that marks a unique object. Objects instantiated with this attached only allows one to exist.
    /// Trying to create/instantiate more copies will have the object destroyed instantly. </summary>
    [DisallowMultipleComponent]
    public sealed class UniqueObject : MonoBehaviour {

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
                Log.Info(
                    "[Unique Object] {0} (ID: {1}) already exists. Destroying {2}",
                    _allIds[ID].name,
                    ID,
                    name);
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
        void Reset() {
            _id = new Random().Next();
        }
    }
}
