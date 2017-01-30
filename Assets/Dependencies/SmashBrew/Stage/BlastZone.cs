using HouraiTeahouse.SmashBrew.Characters;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Stage {

    /// <summary> The Blast Zone script. Publishes PlayerDieEvents in response to Players leaving it's bounds. </summary>
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("Smash Brew/Stage/Blast Zone")]
    public sealed class BlastZone : NetworkBehaviour {

        Collider _col;
        Mediator _eventManager;

        /// <summary> Unity Callback. Called on object instantiation. </summary>
        void Awake() {
            _eventManager = Mediator.Global;
            _col = GetComponent<Collider>();
            // Make sure that the colliders are triggers
            foreach (Collider col in gameObject.GetComponents<Collider>())
                col.isTrigger = true;
        }

        /// <summary> Unity Callback. Called on Trigger Collider entry. </summary>
        /// <param name="other"> the other collider that entered the c </param>
        void OnTriggerExit(Collider other) {
            // Filter only for player characters
            var character = other.GetComponentInParent<Character>();
            var networkIdentity = other.GetComponent<NetworkIdentity>();
            if (!isServer || character == null || networkIdentity == null)
                return;
            Player player = Player.Get(other);
            if (player == null)
                return;

            Vector3 position = other.transform.position;
            if (_col.ClosestPointOnBounds(position) == position)
                return;
            
            player.PlayerObject.SetActive(false);
            _eventManager.Publish(new PlayerDieEvent { Player = player, Revived = false });
        }

    }

}
