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

using HouraiTeahouse.Events;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    /// <summary> The Blast Zone script. Publishes PlayerDieEvents in response to Players leaving it's bounds. </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class BlastZone : MonoBehaviour {
        Collider _col;
        Mediator _eventManager;

        /// <summary> Unity Callback. Called on object instantiation. </summary>
        void Awake() {
            _eventManager = GlobalMediator.Instance;
            _col = GetComponent<Collider>();
            // Make sure that the colliders are triggers
            foreach (Collider col in gameObject.GetComponents<Collider>())
                col.isTrigger = true;
        }

        /// <summary> Unity Callback. Called on Trigger Collider entry. </summary>
        /// <param name="other"> the other collider that entered the c </param>
        void OnTriggerExit(Collider other) {
            // Filter only for player characters
            Player player = Player.GetPlayer(other);
            if (player == null)
                return;

            Vector3 position = other.transform.position;
            if (_col.ClosestPointOnBounds(position) == position)
                return;

            _eventManager.Publish(new PlayerDieEvent {
                Player = player,
                Revived = false
            });
        }
    }
}