using UnityEngine;

namespace HouraiTeahouse {
    /// <summary>
    /// Extension methods for the UnityEngine Vector structs.
    /// </summary>
    public static class VectorExtensions {
        /// <summary>
        /// Computes the Hadamard product between two 2D vectors.
        /// </summary>
        /// <param name="a">the first vector</param>
        /// <param name="b">the second vector</param>
        /// <returns>the Hadamard product of the two vectors</returns>
        public static Vector2 Mult(this Vector2 a, params Vector2[] b) {
            for (var i = 0; i < b.Length; i++)
                a = Vector2.Scale(a, b[i]);
            return a;
        }

        /// <summary>
        /// Computes the Hadamard product between two 3D vectors.
        /// </summary>
        /// <param name="a">the first vector</param>
        /// <param name="b">the second vector</param>
        /// <returns>the Hadamard product of the two vectors</returns>
        public static Vector3 Mult(this Vector3 a, params Vector3[] b) {
            for (var i = 0; i < b.Length; i++)
                a = Vector3.Scale(a, b[i]);
            return a;
        }

        /// <summary>
        /// Computes the Hadamard product between two 4D vectors.
        /// </summary>
        /// <param name="a">the first vector</param>
        /// <param name="b">the second vector</param>
        /// <returns>the Hadamard product of the two vectors</returns>
        public static Vector4 Mult(this Vector4 a, params Vector4[] b) {
            for (var i = 0; i < b.Length; i++)
                a = Vector4.Scale(a, b[i]);
            return a;
        }

        /// <summary>
        /// Computes the largest component of a 2D vector.
        /// </summary>
        /// <param name="v">the source vector</param>
        /// <returns>the largest componet's value</returns>
        public static float Max(this Vector2 v) {
            return Mathf.Max(v.x, v.y);
        }

        /// <summary>
        /// Computes the largest component of a 2D vector.
        /// </summary>
        /// <param name="v">the source vector</param>
        /// <returns>the largest componet's value</returns>
        public static float Max(this Vector3 v) {
            return Mathf.Max(v.x, v.y, v.z);
        }

        /// <summary>
        /// Computes the largest component of a 2D vector.
        /// </summary>
        /// <param name="v">the source vector</param>
        /// <returns>the largest componet's value</returns>
        public static float Max(this Vector4 v) {
            return Mathf.Max(v.x, v.y, v.z, v.w);
        }

        /// <summary>
        /// Computes the smallest component of a 2D vector.
        /// </summary>
        /// <param name="v">the source vector</param>
        /// <returns>the smallest componet's value</returns>
        public static float Min(this Vector2 v) {
            return Mathf.Min(v.x, v.y);
        }

        /// <summary>
        /// Computes the smallest component of a 2D vector.
        /// </summary>
        /// <param name="v">the source vector</param>
        /// <returns>the smallest componet's value</returns>
        public static float Min(this Vector3 v) {
            return Mathf.Min(v.x, v.y, v.z);
        }

        /// <summary>
        /// Computes the smallest component of a 2D vector.
        /// </summary>
        /// <param name="v">the source vector</param>
        /// <returns>the smallest componet's value</returns>
        public static float Min(this Vector4 v) {
            return Mathf.Min(v.x, v.y, v.z, v.w);
        }
    }
}
