using UnityEngine;

namespace Genso.API {

    [RequireComponent(typeof(Collider))]
    public sealed class BlastZone : Singleton<BlastZone> {

        [SerializeField]
        private string[] validCharacterStrings = new string[] { "Character" };


        void Awake() {
            var col = GetComponent<Collider>();
            var rigBod = GetComponent<Rigidbody>();

            // Make sure that the collider isn't a trigger
            col.isTrigger = false;
        }


        void OnTriggerExit(Collider other) {
            // Filter only for player characters
            if (!other.CompareTags(validCharacterStrings))
                return;

            // Find a character script somewhere in the hiearchy
            Character characterScript = other.GetComponentInParent<Character>() ??
                                        other.GetComponentInChildren<Character>();

            if (characterScript == null) {
                Debug.Log("Was expecting" + other.name + " to be a Player, but no Character script was found.");
                return;
            }

            characterScript.BlastZoneExit();
        }

    }

}

