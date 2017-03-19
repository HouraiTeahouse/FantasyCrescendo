using HouraiTeahouse.SmashBrew.Characters;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Stage {

    [AddComponentMenu("SmashBrew/Stage/Ledge")]
    public class StageLedge : NetworkBehaviour {

        [SerializeField]
        bool _direction;

        public bool Direction
        {
            get { return _direction; }
        }

        [SyncVar, SerializeField]
        bool _occupied;

        public bool Occupied {
            get { return _occupied; }
            private set { _occupied = value; }
        }

        public NetworkIdentity GrabberId { get; private set; }

        void OnTriggerEnter(Collider collider) {
            if (!collider.CompareTag(Config.Tags.LedgeTag))
                return;
            var movement = collider.GetComponentInParent<MovementState>();
            if (movement == null)
                return;
            movement.tryLedgeGrab(this);
        }

        public void Grab(MovementState movement) {
            if (Occupied) {
                Log.Debug("Edge Guarded");
                return;
            }
            Occupied = true;
            movement.CurrentLedge = gameObject;
            movement.Direction = _direction;
        }
        
        public void Release(MovementState movement) {
            movement.CurrentLedge = null;
            Occupied = false;
        }
    }

}

