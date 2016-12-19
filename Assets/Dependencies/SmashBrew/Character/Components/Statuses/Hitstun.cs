using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    /// <summary> A Status effect that causes Characters to become uncontrollable for a short period after being hit </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class Hitstun : Status {

        Vector3 _oldVelocity = Vector3.zero;

        /// <summary> Gets whether the player has been hit recently </summary>
        public bool IsHit {
            get { return enabled; }
        }

        /// <summary> Unity callback. Called on entry into a physical collision with another object. </summary>
        /// <param name="col"> the collision data </param>
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
        }

    }

}
