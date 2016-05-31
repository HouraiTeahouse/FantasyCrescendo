using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace HouraiTeahouse.SmashBrew {

    public sealed class Ground : AbstractCharacterComponent {

        private HashSet<Collider> _ground = new HashSet<Collider>();
        private bool _collided;

        /// <summary>
        /// Gets whether the Character is currently on solid Ground.
        /// Assumed to be in the air when false.
        /// </summary>
        public bool IsGrounded {
            get {
                return _ground.Count > 0 && Rigidbody.velocity.y <= 0f &&
                       _ground.Any(ground => ground.gameObject.activeInHierarchy);
            }
        }

        public override void OnReset() {
            _ground.Clear();
            _collided = false;
        }

        public static implicit operator bool(Ground gravity) {
            return gravity && gravity.IsGrounded;
        }

        void OnCollisionEnter(Collision col) {
            GroundCheck(col);
            _collided = true;
        }

        void OnCollisionStay(Collision col) {
            GroundCheck(col);
            _collided = true;
        }

        void OnCollisionExit(Collision col) {
            _ground.Remove(col.collider);
            _collided = false;
        }

        void GroundCheck(Collision collison) {
            ContactPoint[] points = collison.contacts;
            if (points.Length <= 0)
                return;
            var movementCollider = Character.MovementCollider;
            Assert.IsNotNull(movementCollider);
            float r2 = movementCollider.radius * movementCollider.radius;
            Vector3 bottom = transform.TransformPoint(movementCollider.center - Vector3.up * movementCollider.height / 2);
            foreach (ContactPoint contact in points)
                if ((contact.point - bottom).sqrMagnitude < r2)
                    _ground.Add(contact.otherCollider);
        }
    }
}
