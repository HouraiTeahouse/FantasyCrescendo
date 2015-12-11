using UnityEngine;
using Hourai.Events;

namespace Hourai.SmashBrew {

    public class PlayerDieEvent {

        public Player Player;

    }

    [RequireComponent(typeof (Collider))]
    public sealed class BlastZone : MonoBehaviour {

        private Collider _col;
        private Mediator _eventManager;
        private PlayerDieEvent _event;

        /// <summary>
        /// Unity Callback. Called on object instantiation.
        /// </summary>
        void Awake() {
            _eventManager = GlobalMediator.Instance;

            _col = GetComponent<Collider>();
            
            _event = new PlayerDieEvent();

            // Make sure that the colliders are triggers
            foreach(Collider col in gameObject.GetComponents<Collider>())
                col.isTrigger = true;
        }

        /// <summary>
        /// Unity Callback. Called on Trigger Collider entry.
        /// </summary>
        /// <param name="other">the other collider that entered the c</param>
        void OnTriggerExit(Collider other) {
            // Filter only for player characters
            Player player = Player.GetPlayer(other);
            if(player == null)
                return;

            Vector3 position = other.transform.position;
            if (_col.ClosestPointOnBounds(position) == position)
                return;

            _event.Player = player;

            _eventManager.Publish(_event);
        }

    }

}