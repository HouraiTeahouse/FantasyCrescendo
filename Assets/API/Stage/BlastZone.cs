using UnityEngine;

namespace Crescendo.API {

    [RequireComponent(typeof(Collider))]
    public sealed class BlastZone : Singleton<BlastZone> {

        private Collider col;

        protected override void Awake() {
            base.Awake();

            col = GetComponent<Collider>();

            // Make sure that the collider isn't a trigger
            col.isTrigger = true;
        }


        void OnTriggerExit(Collider other) {
            // Filter only for player characters
            if (!other.CompareTag(Game.PlayerTag))
                return;

            Character characterScript = other.GetComponentInParent<Character>() ??
                                        other.GetComponentInChildren<Character>();

            if (characterScript == null) {
                Debug.Log("Was expecting" + other.name + " to be a Player, but no Character script was found.");
                return;
            }

            if (col.ClosestPointOnBounds(characterScript.position) == characterScript.position)
                return;

            characterScript.BlastZoneExit();
        }

    }

}

