using UnityEngine;
using System.Collections;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class Hitstun : Status {
        
        private Vector3 _oldVelocity;
        private Rigidbody _rigidbody;

        public bool IsHit {
            get { return enabled; }
        }

        void Awake() {
            _rigidbody = GetComponent<Rigidbody>();
        }

        protected override void OnStatusUpdate(float dt) {
            _oldVelocity = _rigidbody.velocity;
        }

        protected virtual void OnCollisionEnter(Collision col) {
            if (!IsHit)
                return;

            ContactPoint[] points = col.contacts;
            if (points.Length <= 0)
                return;
            
            Vector3 point = points[0].point;
            Vector3 normal = points[0].normal;
            Vector3 reflection = _oldVelocity - 2 * Vector2.Dot(_oldVelocity, normal) * normal;
            Debug.DrawRay(point, reflection, Color.green);
            Debug.DrawRay(point, normal, Color.red);
            Debug.DrawRay(point, -_oldVelocity, Color.yellow);
            _rigidbody.velocity = Vector3.ClampMagnitude(reflection, 0.8f * _oldVelocity.magnitude);
        }

    }

}
