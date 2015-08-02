using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Crescendo.API {

    [RequireComponent(typeof(Collider))]
    public class Platform : MonoBehaviour {

        private Collider[] solidColliders;
        private HashSet<Collider> ignored; 
        private Dictionary<Collider, Character> matches; 

        void Awake() {
            ignored = new HashSet<Collider>();
            matches = new Dictionary<Collider, Character>();

            var colliders = new List<Collider>();
            foreach(var collider in GetComponentsInChildren<Collider>())
                if (!collider.isTrigger) {
                    colliders.Add(collider);
                }
            solidColliders = colliders.ToArray();
        }

        void ChangeIgnore(Collider target, bool state) {
            foreach (var col in solidColliders)
                Physics.IgnoreCollision(col, target, state);
        }

        void Check(Collider target)
        {
            if (target == null || !target.CompareTag(Game.PlayerTag))
                return;

            Character character;
            if (matches.ContainsKey(target))
                character = matches[target];
            else {
                character = target.GetComponentInParent<Character>();
                if(character != null)
                    matches[target] = character;
            }

            if (character == null)
                return;

            if (character.Velocity.y > 0 && !ignored.Contains(target)) {
                ignored.Add(target);
                ChangeIgnore(target, true);
            } else if (character.Velocity.y <= 0 && ignored.Contains(target)) {
                ignored.Remove(target);
                ChangeIgnore(target, false);
            }
        }

        void OnTriggerEnter(Collider other) {
            Check(other);
        }

        void OnTriggerStay(Collider other) {
            Check(other);
        }

        void OnTriggerExit(Collider other) {
            if (other.CompareTag(Game.PlayerTag) && ignored.Contains(other)) {
                ignored.Remove(other);
                ChangeIgnore(other, false);
            }
        }

    }

}

    