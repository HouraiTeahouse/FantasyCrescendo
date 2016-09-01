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

using System.Linq;
using UnityEngine;

namespace HouraiTeahouse {

    /// <summary> Extension methods for the UnityEngine Vector structs. </summary>
    public static class VectorExtensions {

        /// <summary> Computes the Hadamard product between two 2D vectors. </summary>
        /// <param name="a"> the first vector </param>
        /// <param name="b"> the second vector </param>
        /// <returns> the Hadamard product of the two vectors </returns>
        public static Vector2 Mult(this Vector2 a, params Vector2[] b) {
            return b.Aggregate(a, Vector2.Scale);
        }

        /// <summary> Computes the Hadamard product between two 3D vectors. </summary>
        /// <param name="a"> the first vector </param>
        /// <param name="b"> the second vector </param>
        /// <returns> the Hadamard product of the two vectors </returns>
        public static Vector3 Mult(this Vector3 a, params Vector3[] b) {
            return b.Aggregate(a, Vector3.Scale);
        }

        /// <summary> Computes the Hadamard product between two 4D vectors. </summary>
        /// <param name="a"> the first vector </param>
        /// <param name="b"> the second vector </param>
        /// <returns> the Hadamard product of the two vectors </returns>
        public static Vector4 Mult(this Vector4 a, params Vector4[] b) {
            return b.Aggregate(a, Vector4.Scale);
        }

        /// <summary> Computes the largest component of a 2D vector. </summary>
        /// <param name="v"> the source vector </param>
        /// <returns> the largest componet's value </returns>
        public static float Max(this Vector2 v) {
            return Mathf.Max(v.x, v.y);
        }

        /// <summary> Computes the largest component of a 2D vector. </summary>
        /// <param name="v"> the source vector </param>
        /// <returns> the largest componet's value </returns>
        public static float Max(this Vector3 v) {
            return Mathf.Max(v.x, v.y, v.z);
        }

        /// <summary> Computes the largest component of a 2D vector. </summary>
        /// <param name="v"> the source vector </param>
        /// <returns> the largest componet's value </returns>
        public static float Max(this Vector4 v) {
            return Mathf.Max(v.x, v.y, v.z, v.w);
        }

        /// <summary> Computes the smallest component of a 2D vector. </summary>
        /// <param name="v"> the source vector </param>
        /// <returns> the smallest componet's value </returns>
        public static float Min(this Vector2 v) {
            return Mathf.Min(v.x, v.y);
        }

        /// <summary> Computes the smallest component of a 2D vector. </summary>
        /// <param name="v"> the source vector </param>
        /// <returns> the smallest componet's value </returns>
        public static float Min(this Vector3 v) {
            return Mathf.Min(v.x, v.y, v.z);
        }

        /// <summary> Computes the smallest component of a 2D vector. </summary>
        /// <param name="v"> the source vector </param>
        /// <returns> the smallest componet's value </returns>
        public static float Min(this Vector4 v) {
            return Mathf.Min(v.x, v.y, v.z, v.w);
        }
    }
}
