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

namespace HouraiTeahouse {
    /// <summary> A MonoBehaviour that restricts the posiiton of all of the children of the GameObject it is attached to. </summary>
    public class RestrainChildren : MonoBehaviour {
        /// <summary> In local coordiates, the bounds for where the children of the GameObject can move </summary>
        [SerializeField]
        Bounds _bounds;

        /// <summary> Unity callback. Called once per frame after all Update calls. </summary>
        void LateUpdate() {
            foreach (Transform child in transform)
                child.localPosition = _bounds.ClosestPoint(child.localPosition);
        }

        /// <summary> Unity callback. Called in the editor to draw gizmos in the scene view. </summary>
        void OnDrawGizmos() {
            using (GizmoUtil.With(Color.white, transform)) {
                Gizmos.DrawWireCube(_bounds.center, _bounds.size);
            }
        }
    }
}