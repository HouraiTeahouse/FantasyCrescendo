using HouraiTeahouse.SmashBrew.Characters;
using UnityConstants;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    public class StageLedge : MonoBehaviour {

        [SerializeField]
        bool _direction;

        public bool Occupied { get; private set; }

        void OnTriggerEnter(Collider collider) {
            if (!collider.CompareTag(Tags.Player))
                return;
            Log.Debug("Ledge Collided");
            var movement = collider.GetComponentInParent<MovementState>();
            if (movement == null)
                return;
            Log.Debug("Ledge Touched");
            movement.Direction = _direction;
            movement.CurrentLedge = transform;
            //TODO(james7132): Implement
        }

    }

}

