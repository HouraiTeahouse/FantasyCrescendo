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
        public static Vector2 Mult(this Vector2 a, Vector2 b) {
            return new Vector2(a.x * b.x, a.y * b.y);
        }

        /// <summary>
        /// Computes the Hadamard product between two 3D vectors.
        /// </summary>
        /// <param name="a">the first vector</param>
        /// <param name="b">the second vector</param>
        /// <returns>the Hadamard product of the two vectors</returns>
        public static Vector3 Mult(this Vector3 a, Vector3 b) {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        /// <summary>
        /// Computes the Hadamard product between two 4D vectors.
        /// </summary>
        /// <param name="a">the first vector</param>
        /// <param name="b">the second vector</param>
        /// <returns>the Hadamard product of the two vectors</returns>
       public static Vector4 Mult(this Vector4 a, Vector4 b) {
            return new Vector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
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
