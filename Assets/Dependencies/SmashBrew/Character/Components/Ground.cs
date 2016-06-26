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

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.SmashBrew {
    [Required]
    public sealed class Ground : CharacterComponent {
        readonly HashSet<Collider> _ground = new HashSet<Collider>();
        bool _collided;

        /// <summary> Gets whether the Character is currently on solid Ground. Assumed to be in the air when false. </summary>
        public bool IsGrounded {
            get {
                return _ground.Count > 0 && Rigidbody.velocity.y <= 0f
                    && _ground.Any(ground => ground.gameObject.activeInHierarchy);
            }
        }

        public override void OnReset() {
            _ground.Clear();
            _collided = false;
        }

        public static implicit operator bool(Ground ground) {
            return ground != null && ground.IsGrounded;
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
            CapsuleCollider movementCollider = Character.MovementCollider;
            Assert.IsNotNull(movementCollider);
            float r2 = movementCollider.radius * movementCollider.radius;
            Vector3 bottom =
                transform.TransformPoint(movementCollider.center
                    - Vector3.up * movementCollider.height / 2);
            foreach (ContactPoint contact in points)
                if ((contact.point - bottom).sqrMagnitude < r2)
                    _ground.Add(contact.otherCollider);
        }
    }
}
