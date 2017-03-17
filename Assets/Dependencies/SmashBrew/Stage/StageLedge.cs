using HouraiTeahouse.SmashBrew.Characters;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Stage {

    [AddComponentMenu("SmashBrew/Stage/Ledge")]
    public class StageLedge : MonoBehaviour {

        [SerializeField]
        bool _direction;

        public bool Occupied { get; private set; }

        void OnTriggerEnter(Collider collider) {
            if (!collider.CompareTag(Config.Tags.LedgeTag))
                return;
            var movement = collider.GetComponentInParent<MovementState>();
            if (movement == null || Occupied)
                return;
            Grab(movement);
        }

        public void Grab(MovementState movement)
        {
            Log.Debug("Grabbed Ledge");
            Occupied = true;
            movement.Direction = _direction;
            movement.CurrentLedge = transform;
        }

        public void Release()
        {
            Log.Debug("Released Ledge");
            Occupied = false;
        }
    }

}

