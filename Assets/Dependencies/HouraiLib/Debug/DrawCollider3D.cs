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

    /// <summary> Draws Colliders as Gizmos, permanentally seen in the Scene view. Good for general establishing of boundaries.
    /// Currently does not support CapsuleColliders </summary>
    public class DrawCollider3D : MonoBehaviour {

        [SerializeField]
        [Tooltip("The color used to draw the colliders with.")]
        Color color;

        [SerializeField]
        [Tooltip("Whether or not to include the Colliders in the children of the GameObject or not.")]
        bool includeChildren;

        [SerializeField]
        [Tooltip("If set to true, colliders are drawn as solids, otherwise drawn as wireframes.")]
        bool solid;

#if UNITY_EDITOR
        /// <summary> Unity Callback. Called in the Editor to draw Gizmos on each GUI update. </summary>
        void OnDrawGizmos() {
            Collider[] colliders = includeChildren
                ? GetComponentsInChildren<Collider>()
                : GetComponents<Collider>();

            if (colliders == null)
                return;

            Gizmos.DrawColliders(colliders, color, solid);
        }
#endif

    }
}
