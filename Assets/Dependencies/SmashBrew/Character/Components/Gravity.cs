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

    [RequireComponent(typeof(Ground), typeof(Rigidbody))]
    public sealed class Gravity : MonoBehaviour {
        [SerializeField]
        [Tooltip("The acceleration downward per second applied")]
        float _gravity = 9.86f;

        /// <summary> Gets or sets the magnitude of gravity applied to the Character. </summary>
        public float Force {
            get { return _gravity; }
            set { _gravity = Mathf.Abs(value); }
        }

        public static implicit operator float(Gravity gravity) {
            return gravity != null ? gravity.Force : 0f;
        }

        Ground _ground;
        Rigidbody _rigidbody;

        void Awake() {
            _ground = GetComponent<Ground>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        void FixedUpdate() {
            float grav = Force;
            //Simulates ground friction.
            if (_ground)
                grav *= 2.5f;
            _rigidbody.AddForce(-Vector3.up * grav, ForceMode.Acceleration);
        }
    }
}
