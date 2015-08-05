using System.Collections.Generic;
using UnityEngine;

namespace Crescendo.API {

    public abstract class TriggerStageElement : MonoBehaviour {

        private Collider[] _toIgnore;

        private void Awake() {
            var colliders = new List<Collider>();
            foreach (var col in GetComponentsInChildren<Collider>()) {
                if (!col.isTrigger)
                    colliders.Add(col);
            }
            _toIgnore = colliders.ToArray();
        }

        protected void ChangeIgnore(Collider target, bool state) {
            if (target == null || !target.CompareTag(Game.PlayerTag))
                return;

            foreach (var col in _toIgnore)
                Physics.IgnoreCollision(col, target, state);
        }

    }

    public class Platform : TriggerStageElement {

        private void OnTriggerEnter(Collider other) {
            ChangeIgnore(other, true);
        }

        private void OnTriggerExit(Collider other) {
            ChangeIgnore(other, false);
        }

    }

}