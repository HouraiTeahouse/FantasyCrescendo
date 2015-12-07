using UnityEngine;
using System.Collections;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class Hitstun : Status {
        
        private Vector3 _oldVelocity;

        public bool IsHit {
            get { return enabled; }
        }

        protected override void OnStatusUpdate(float dt) {
            _oldVelocity = Rigidbody.velocity;
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
            Rigidbody.velocity = Vector3.ClampMagnitude(reflection, 0.8f * _oldVelocity.magnitude);
        }

    }

}
