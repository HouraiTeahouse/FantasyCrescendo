using UnityEngine;
using System;

namespace Hourai.SmashBrew {

    [RequireComponent(typeof (Collider))]
    public sealed class BlastZone : Singleton<BlastZone> {

        private Collider col;
        public event Action<Character> OnBlastZoneExit;

        protected override void Awake() {
            base.Awake();

            col = GetComponent<Collider>();
            
            // Make sure that the collider isn't a trigger
            col.isTrigger = true;
        }

        private void OnTriggerExit(Collider other) {
            // Filter only for player characters
            if (OnBlastZoneExit == null || !SmashGame.IsPlayer(other))
                return;

            Character characterScript = other.transform.root.GetComponentInChildren<Character>();

            if (characterScript == null) {
                Debug.Log("Was expecting" + other.name + " to be a HumanPlayer, but no Character script was found.");
                return;
            }

            Vector3 position = characterScript.transform.position;
            if (col.ClosestPointOnBounds(position) == position)
                return;

            OnBlastZoneExit(characterScript);
        }

    }

}