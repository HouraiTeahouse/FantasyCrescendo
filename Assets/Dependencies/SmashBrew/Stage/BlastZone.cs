using UnityEngine;
using System;
using Hourai.Events;

namespace Hourai.SmashBrew {

    public class BlastZoneExit : IEvent {

        public Character player;

    }

    [RequireComponent(typeof (Collider))]
    public sealed class BlastZone : Singleton<BlastZone> {

        private Collider col;
        private Mediator eventManager;

        protected override void Awake() {
            base.Awake();
            eventManager = GlobalEventManager.Instance;

            col = GetComponent<Collider>();
            
            // Make sure that the collider isn't a trigger
            col.isTrigger = true;
        }

        private void OnTriggerExit(Collider other) {
            // Filter only for player characters
            if(!Game.IsPlayer(other))
                return;

            var characterScript = other.transform.root.GetComponentInChildren<Character>();

            if (characterScript == null) {
                Debug.Log("Was expecting" + other.name + " to be a HumanPlayer, but no Character script was found.");
                return;
            }

            Vector3 position = characterScript.transform.position;
            if (col.ClosestPointOnBounds(position) == position)
                return;

            eventManager.Publish(new BlastZoneExit { player = characterScript });
        }

    }

}