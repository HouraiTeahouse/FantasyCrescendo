using UnityEngine;

namespace Hourai.SmashBrew {

    [RequireComponent(typeof (Collider))]
    public sealed class BlastZone : Singleton<BlastZone> {

        private Collider col;

        protected override void Awake() {
            base.Awake();

            col = GetComponent<Collider>();

            // Make sure that the collider isn't a trigger
            col.isTrigger = true;
        }

        private void OnTriggerExit(Collider other) {
            // Filter only for player characters
            if (!Game.IsPlayer(other))
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