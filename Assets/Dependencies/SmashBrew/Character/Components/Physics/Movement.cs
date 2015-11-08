using System;
using UnityEngine;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    [RequireComponent(typeof(Rigidbody), typeof(Grounding))]
    public class Movement : RestrictableCharacterComponent {

        private Rigidbody _rigidbody;
        private Grounding _ground;
        private Facing _facing;

        void Awake() {
            _rigidbody = GetComponent<Rigidbody>();
            _ground = GetComponent<Grounding>();
            _facing = GetComponent<Facing>();
        }

        public void Move(float speed) {
            if (Restricted)
                return;

            Vector3 vel = _rigidbody.velocity;

            vel.x = speed;

            if (_facing && _facing.Direction)
                vel.x *= -1;

            _rigidbody.velocity = vel;
        }

    }

}