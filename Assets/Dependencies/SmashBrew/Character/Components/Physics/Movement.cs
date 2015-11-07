using System;
using UnityEngine;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    [RequireComponent(typeof(Rigidbody), typeof(Grounding), typeof(Facing))]
    public class Movement : RestrictableCharacterComponent {

        private Rigidbody _rigidbody;
        private Facing _facing;

        void Awake() {
            _rigidbody = GetComponent<Rigidbody>();
            _facing = GetComponent<Facing>();
        }

        public void Move(float speed) {
            if (Restricted)
                return;

            Vector3 vel = _rigidbody.velocity;

            vel.x = speed;

            if (_facing.Direction)
                speed *= -1;

            _rigidbody.velocity = vel;
        }

    }

}