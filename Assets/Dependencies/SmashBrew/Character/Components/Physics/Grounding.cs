using UnityEngine;
using System;
using System.Collections.Generic;
using Hourai.Events;

namespace Hourai.SmashBrew {

    public class GroundEvent : IEvent {

        public bool grounded;

    }

    [RequiredCharacterComponent]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CapsuleCollider), typeof(Animator))]
    public class Grounding : CharacterComponent {
        
        private CapsuleCollider _movementCollider;

        private HashSet<Collider> ground;

        public bool IsGrounded {
            get { return Animator.GetBool(CharacterAnimVars.Grounded); }
            private set {
                if (IsGrounded == value)
                    return;
                Animator.SetBool(CharacterAnimVars.Grounded, value);
                CharacterEvents.Publish(new GroundEvent { grounded = value });
            }
        }

        void Awake() {
            ground = new HashSet<Collider>();
            _movementCollider = GetComponent<CapsuleCollider>();
        }

        void OnCollisionEnter(Collision col) {
            ContactPoint[] points = col.contacts;
            if (points.Length <= 0)
                return;

            float r2 = _movementCollider.radius * _movementCollider.radius;
            Vector3 bottom = transform.TransformPoint(_movementCollider.center - Vector3.up * _movementCollider.height / 2);
            for (var i = 0; i < points.Length; i++)
                if ((points[i].point - bottom).sqrMagnitude < r2)
                    ground.Add(points[i].otherCollider);
            IsGrounded = ground.Count > 0;
        }

        protected virtual void OnCollisionExit(Collision col) {
            if (ground.Remove(col.collider))
                IsGrounded = ground.Count > 0;
        }
    }

}

