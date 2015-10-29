using System;
using UnityEngine;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    [RequireComponent(typeof(Rigidbody), typeof(Grounding))]
    public class Movement : RestrictableCharacterComponent {

        [SerializeField]
        private float _airSpeed = 3f;

        [SerializeField]
        private float _dashSpeed = 5f;

        [SerializeField]
        private float _walkSpeed = 3f;

        private Rigidbody _rigidbody;
        private Grounding _ground;

        void Awake() {
            _rigidbody = GetComponent<Rigidbody>();
            _ground = GetComponent<Grounding>();
        }

        public void Move(Vector2 direction) {
            if (Restricted || Mathf.Abs(direction.x) < float.Epsilon)
                return;

            Vector3 vel = _rigidbody.velocity;

            if (_ground.IsGrounded) {
               // if (Character.IsDashing)
               //     vel.x = _dashSpeed;
               // else
                    vel.x = _walkSpeed;
            } else
                vel.x = _airSpeed;
            vel.x = Util.MatchSign(vel.x, direction.x);

            _rigidbody.velocity = vel;
        }

    }

}