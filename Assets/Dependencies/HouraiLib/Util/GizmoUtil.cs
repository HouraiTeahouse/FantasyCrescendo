using System;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse {

    sealed class GizmoDisposable : IDisposable {

        private readonly Color? _oldColor;
        private readonly Matrix4x4? _oldTransform;

        public GizmoDisposable(Color? color, Matrix4x4? matrix) {
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


    /// <summary>
    /// A static utlity class of functions for helping draw Gizmos
    /// </summary>
    public static class GizmoUtil {

        /// <summary>
        /// Creates an IDisposable object for drawing Gizmos of a certain color.
        /// </summary>
        /// <example>
        /// using(GizmoUtil.With(Color.white) {
        ///     // Draw a white cube
        ///     Gizmos.DrawCube(center, size); 
        /// }
        /// </example>
        /// <param name="color">the color to set the Gizmos color to</param>
        /// <returns>the IDisposable object for working with Gizmos</returns>
        public static IDisposable With(Color color) {
            return new GizmoDisposable(color, null);
        }

        /// <summary>
        /// Creates an IDisposable object for drawing Gizmos within certain transformation.
        /// </summary>
        /// <param name="transform">the transformation matrix to use when drawing Gizmos</param>
        /// <returns>the IDisposable object for working with Gizmos</returns>
        public static IDisposable With(Matrix4x4 transform) {
            return new GizmoDisposable(null, transform);
        }

        /// <summary>
        /// Creates an IDisposable object for drawing Gizmos within certain transformation and color.
        /// </summary>
        /// <param name="color">the color to set the Gizmos color to</param>
        /// <param name="transform">the transformation matrix to use when drawing Gizmos</param>
        /// <returns>the IDisposable object for working with Gizmos</returns>
        public static IDisposable With(Color color, Matrix4x4 transform) {
            return new GizmoDisposable(color, transform);
        }

        /// <summary>
        /// Creates an IDisposable object for drawing Gizmos within certain transformation.
        /// </summary>
        /// <param name="transform">the transformation matrix to use when drawing Gizmos</param>
        /// <returns>the IDisposable object for working with Gizmos</returns>
        public static IDisposable With(Transform transform) {
            if (transform == null)
                throw new ArgumentNullException("transform");
            return new GizmoDisposable(null, transform.localToWorldMatrix);
        }

        /// <summary>
        /// Creates an IDisposable object for drawing Gizmos within certain transformation and color.
        /// </summary>
        /// <param name="color">the color to set the Gizmos color to</param>
        /// <param name="transform">the transformation matrix to use when drawing Gizmos</param>
        /// <returns>the IDisposable object for working with Gizmos</returns>
        public static IDisposable With(Color color, Transform transform) {
            if (transform == null)
                throw new ArgumentNullException("transform");
            return new GizmoDisposable(color, transform.localToWorldMatrix);
        }

        /// <summary>
        /// Draws a Gizmo that matches the shape and size of a given 3D collider
        /// </summary>
        /// <param name="collider">the collider to draw</param>
        /// <param name="color">the color to draw the collider in</param>
        /// <param name="solid">if true, draws a a solid gizmo, otherwise draws it as a wire mesh</param>
        public static void DrawCollider(Collider collider, Color color, bool solid = false) {
            if (collider == null)
                return;
            using (With(color, collider.transform))
                DrawCollider3D_Impl(collider, solid);
        }

        /// <summary>
        /// Draws Gizmos that matches the shape and size of multiple 3D colliders 
        /// </summary>
        /// <param name="colliders">the colliders to draw</param>
        /// <param name="color">the color to draw the collider in</param>
        /// <param name="solid">if true, draws a a solid gizmo, otherwise draws it as a wire mesh</param>
        /// <param name="filter">a check for whether to draw a collider or not, if null, all colliders are drawn</param>
        public static void DrawColliders(IEnumerable<Collider> colliders,
                                           Color color,
                                           bool solid = false,
                                           Predicate<Collider> filter = null) {
            if (colliders == null)
                return;

            using (With(color, Matrix4x4.identity)) {
                Collider[] asArray = colliders as Collider[];
                if (asArray != null) {
                    foreach (Collider collider in asArray) {
                        if (collider == null || (filter != null && !filter(collider)))
                            continue;
                        Gizmos.matrix = collider.transform.localToWorldMatrix;
                        DrawCollider3D_Impl(collider, solid);
                    }
                } else {
                    foreach (Collider collider in colliders) {
                        if (collider == null || (filter != null && !filter(collider)))
                            continue;
                        Gizmos.matrix = collider.transform.localToWorldMatrix;
                        DrawCollider3D_Impl(collider, solid);
                    }
                }
            }
        }

        static void DrawCollider3D_Impl(Collider collider, bool solid) {
            var boxCollider = collider as BoxCollider;
            var sphereCollider = collider as SphereCollider;
            var meshCollider = collider as MeshCollider;
            if (solid) {
                if (boxCollider != null)
                    Gizmos.DrawCube(boxCollider.center, boxCollider.size);
                else if (sphereCollider != null)
                    Gizmos.DrawSphere(sphereCollider.center, sphereCollider.radius);
                else if (meshCollider != null)
                    Gizmos.DrawMesh(meshCollider.sharedMesh, Vector3.zero);
            } else {
                if (boxCollider != null)
                    Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
                else if (sphereCollider != null)
                    Gizmos.DrawWireSphere(sphereCollider.center, sphereCollider.radius);
                else if (meshCollider != null)
                    Gizmos.DrawWireMesh(meshCollider.sharedMesh, Vector3.zero);
            }
        }

    }

}
