// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    /// <summary> A Status effect that causes Characters to become uncontrollable for a short period after being hit </summary>
    [Required]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class Hitstun : Status {
        Vector3 _oldVelocity;

        /// <summary> Gets whether the player has been hit recently </summary>
        public bool IsHit {
            get { return enabled; }
        }

        /// <summary>
        ///     <see cref="Status.OnStatusUpdate" />
        /// </summary>
        protected override void OnStatusUpdate(float dt) {
            _oldVelocity = Rigidbody.velocity;
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
            Rigidbody.velocity = Vector3.ClampMagnitude(reflection,
                0.8f * _oldVelocity.magnitude);
        }
    }
}
