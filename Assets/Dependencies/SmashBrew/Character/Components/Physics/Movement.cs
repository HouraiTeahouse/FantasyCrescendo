using System;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    [RequireComponent(typeof(Rigidbody), typeof(Grounding), typeof(Facing))]
    public class Movement : RestrictableCharacterComponent {
        
        private Facing _facing;

        protected override void Awake() {
            base.Awake();
            _facing = GetComponent<Facing>();
        }

        public void Move(float speed) {
            if (Restricted)
                return;

            Vector3 vel = Rigidbody.velocity;

            vel.x = speed;

            if (_facing && _facing.Direction)
                vel.x *= -1;

            Rigidbody.velocity = vel;
        }

    }

}
