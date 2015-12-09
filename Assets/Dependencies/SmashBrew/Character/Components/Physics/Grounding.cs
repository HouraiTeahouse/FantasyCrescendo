using UnityEngine;
using System.Collections.Generic;
using Hourai.Events;

namespace Hourai.SmashBrew {

    public class GroundEvent {

        public bool Grounded;

    }

    [RequiredCharacterComponent]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CapsuleCollider), typeof(Animator))]
    public class Grounding : CharacterComponent {
        
        private CapsuleCollider _movementCollider;

        private HashSet<Collider> _ground;

        public bool IsGrounded {
            get { return Animator.GetBool(CharacterAnimVars.Grounded); }
            private set {
                if (IsGrounded == value)
                    return;
                Animator.SetBool(CharacterAnimVars.Grounded, value);
                CharacterEvents.Publish(new GroundEvent { Grounded = value });
            }
        }

        protected override void Awake() {
            base.Awake();
            _ground = new HashSet<Collider>();
            _movementCollider = GetComponent<CapsuleCollider>();
        }

        void OnCollisionEnter(Collision col) {
            ContactPoint[] points = col.contacts;
            if (points.Length <= 0)
                return;

            float r2 = _movementCollider.radius * _movementCollider.radius;
            Vector3 bottom = transform.TransformPoint(_movementCollider.center - Vector3.up * _movementCollider.height / 2);
            foreach (ContactPoint contact in points)
                if ((contact.point - bottom).sqrMagnitude < r2)
                    _ground.Add(contact.otherCollider);
            IsGrounded = _ground.Count > 0;
        }

        protected virtual void OnCollisionExit(Collision col) {
            if (_ground.Remove(col.collider))
                IsGrounded = _ground.Count > 0;
        }
    }

}

