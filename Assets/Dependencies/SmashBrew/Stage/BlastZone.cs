using UnityEngine;
using Hourai.Events;

namespace Hourai.SmashBrew {

    public class PlayerDieEvent : IEvent {

        public Player Player;

    }

    [RequireComponent(typeof (Collider))]
    public sealed class BlastZone : Singleton<BlastZone> {

        private Collider _col;
        private Mediator _eventManager;
        private PlayerDieEvent _event;

        protected override void Awake() {
            base.Awake();
            _eventManager = GlobalEventManager.Instance;

            _col = GetComponent<Collider>();
            
            _event = new PlayerDieEvent();

            // Make sure that the collider isn't a trigger
            _col.isTrigger = true;
        }

        private void OnTriggerExit(Collider other) {
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