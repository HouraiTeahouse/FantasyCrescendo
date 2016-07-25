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

using System;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

namespace HouraiTeahouse {
    internal struct GizmoDisposable : IDisposable {
        readonly Color? _oldColor;
        readonly Matrix4x4? _oldTransform;

        public GizmoDisposable(Color? color, Matrix4x4? matrix) {
            _oldColor = null;
            _oldTransform = null;
            if (color != null) {
                _oldColor = Gizmos.color;
                Gizmos.color = color.Value;
            }
            if (matrix != null) {
                _oldTransform = Gizmos.matrix;
                Gizmos.matrix = matrix.Value;
            }
        }

        public void Dispose() {
            if (_oldColor != null)
                Gizmos.color = (Color) _oldColor;
            if (_oldTransform != null)
                Gizmos.matrix = (Matrix4x4) _oldTransform;
        }
    }


    /// <summary> A static utlity class of functions for helping draw Gizmos </summary>
    public static class GizmoUtil {
        /// <summary> Creates an IDisposable object for drawing Gizmos of a certain color. </summary>
        /// <example> using(GizmoUtil.With(Color.white) { // Draw a white cube Gizmos.DrawCube(center, size); } </example>
        /// <param name="color"> the color to set the Gizmos color to </param>
        /// <returns> the IDisposable object for working with Gizmos </returns>
        public static IDisposable With(Color color) {
            return new GizmoDisposable(color, null);
        }

        /// <summary> Creates an IDisposable object for drawing Gizmos within certain transformation. </summary>
        /// <param name="transform"> the transformation matrix to use when drawing Gizmos </param>
        /// <returns> the IDisposable object for working with Gizmos </returns>
        public static IDisposable With(Matrix4x4 transform) {
            return new GizmoDisposable(null, transform);
        }

        /// <summary> Creates an IDisposable object for drawing Gizmos within certain transformation and color. </summary>
        /// <param name="color"> the color to set the Gizmos color to </param>
        /// <param name="transform"> the transformation matrix to use when drawing Gizmos </param>
        /// <returns> the IDisposable object for working with Gizmos </returns>
        public static IDisposable With(Color color, Matrix4x4 transform) {
            return new GizmoDisposable(color, transform);
        }

        /// <summary> Creates an IDisposable object for drawing Gizmos within certain transformation. </summary>
        /// <param name="transform"> the transformation matrix to use when drawing Gizmos </param>
        /// <returns> the IDisposable object for working with Gizmos </returns>
        public static IDisposable With(Transform transform) {
            return new GizmoDisposable(null,
                Check.NotNull(transform).localToWorldMatrix);
        }

        /// <summary> Creates an IDisposable object for drawing Gizmos within certain transformation and color. </summary>
        /// <param name="color"> the color to set the Gizmos color to </param>
        /// <param name="transform"> the transformation matrix to use when drawing Gizmos </param>
        /// <returns> the IDisposable object for working with Gizmos </returns>
        public static IDisposable With(Color color, Transform transform) {
            return new GizmoDisposable(color,
                Check.NotNull(transform).localToWorldMatrix);
        }

        /// <summary> Draws a Gizmo that matches the shape and size of a given 3D col </summary>
        /// <param name="collider"> the col to draw </param>
        /// <param name="color"> the color to draw the col in </param>
        /// <param name="solid"> if true, draws a a solid gizmo, otherwise draws it as a wire mesh </param>
        public static void DrawCollider(Collider collider,
                                        Color color,
                                        bool solid = false) {
            if (collider == null)
                return;
            using (With(color, collider.transform))
                DrawCollider3D_Impl(collider, solid);
        }

        /// <summary> Draws Gizmos that matches the shape and size of multiple 3D colliders </summary>
        /// <param name="colliders"> the colliders to draw </param>
        /// <param name="color"> the color to draw the col in </param>
        /// <param name="solid"> if true, draws a a solid gizmo, otherwise draws it as a wire mesh </param>
        /// <param name="filter"> a check for whether to draw a col or not, if null, all colliders are drawn </param>
        public static void DrawColliders(IEnumerable<Collider> colliders,
                                         Color color,
                                         bool solid = false,
                                         Predicate<Collider> filter = null) {
            if (colliders == null)
                return;

            using (With(color, Gizmos.matrix)) {
                foreach (Collider collider in colliders) {
                    if (collider == null
                        || (filter != null && !filter(collider)))
                        continue;
                    DrawCollider3D_Impl(collider, solid);
                }
            }
        }

        public static Matrix4x4 GetColliderMatrix(Collider col) {
            //Check.NotNull(col);
            //var sphereCollider = col as SphereCollider;
            //Transform transform = col.transform;
            //if (sphereCollider != null)
            //    return Matrix4x4.TRS(transform.TransformPoint(sphereCollider.center),
            //            transform.rotation,
            //            Vector3.one * transform.lossyScale.Max()) * transform.localToWorldMatrix;
            //return transform.localToWorldMatrix;
            var boxCol = col as BoxCollider;
            var sphereCol = col as SphereCollider;
            var capsuleCol = col as CapsuleCollider;
            Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;
            Vector3 scale = Vector3.one;
            Matrix4x4 localToWorld;
            Transform transform = col.transform;
            if (boxCol != null) {
                position = boxCol.center;
                scale = boxCol.size;
                localToWorld = transform.localToWorldMatrix;
            }
            else if (sphereCol != null) {
                scale = sphereCol.radius * Vector3.one;
                localToWorld = Matrix4x4.TRS(transform.TransformPoint(sphereCol.center),
                    transform.rotation,
                    Vector3.one * transform.lossyScale.Max());
            }
            else if (capsuleCol != null) {
                position = capsuleCol.center;
                scale = Vector3.one * capsuleCol.radius * 2;
                scale[capsuleCol.direction] = capsuleCol.height / 2;
                switch (capsuleCol.direction) {
                    case 1:
                        rotation = Quaternion.Euler(0, 90, 0);
                        break;
                    case 2:
                        rotation = Quaternion.Euler(90, 0, 0);
                        break;
                }
                localToWorld = transform.localToWorldMatrix;
            }
            else {
                localToWorld = transform.localToWorldMatrix;
            }
            return localToWorld * Matrix4x4.TRS(position, rotation, scale);
        }

        static void DrawCollider3D_Impl(Collider collider, bool solid) {
            var boxCollider = collider as BoxCollider;
            var sphereCollider = collider as SphereCollider;
            var meshCollider = collider as MeshCollider;
            using (With(GetColliderMatrix(collider))) {
                if (solid) {
                    if (sphereCollider != null)
                        Gizmos.DrawSphere(Vector3.zero, 1f);
                    else {
                        if (boxCollider != null)
                            Gizmos.DrawCube(Vector3.zero, Vector3.one);
                        else if (meshCollider != null)
                            Gizmos.DrawMesh(meshCollider.sharedMesh, Vector3.zero);
                    }
                }
                else {
                    if (sphereCollider != null)
                        Gizmos.DrawWireSphere(Vector3.zero, 1f);
                    else {
                        if (boxCollider != null)
                            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                        else if (meshCollider != null)
                            Gizmos.DrawWireMesh(meshCollider.sharedMesh, Vector3.zero);
                    }
                }
            }
        }
    }
}
