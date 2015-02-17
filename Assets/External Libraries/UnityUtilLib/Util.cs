using UnityEngine;
using System.Collections;

namespace UnityUtilLib
{
	/// <summary>
	/// Util.
	/// </summary>
	public static class Util {

		/// <summary>
		/// The degree2 RAD.
		/// </summary>
		public const float Degree2Rad = Mathf.PI / 180f;

		/// <summary>
		/// The rad2 degree.
		/// </summary>
		public const float Rad2Degree = 180f / Mathf.PI;

		/// <summary>
		/// Sign the specified e.
		/// </summary>
		/// <param name="e">E.</param>
		public static float Sign(float e) {
			return (e == 0f) ? 0f : Mathf.Sign (e);
		}

		/// <summary>
		/// To2s the d.
		/// </summary>
		/// <returns>The d.</returns>
		/// <param name="v">V.</param>
		public static Vector2 To2D(Vector3 v) {
			return new Vector2(v.x, v.y);
		}

		/// <summary>
		/// To3s the d.
		/// </summary>
		/// <returns>The d.</returns>
		/// <param name="v">V.</param>
		/// <param name="z">The z coordinate.</param>
		public static Vector3 To3D(Vector2 v, float z = 0f) {
			return new Vector3 (v.x, v.y, z);
		}

		/// <summary>
		/// Randoms the vect2.
		/// </summary>
		/// <returns>The vect2.</returns>
		/// <param name="v">V.</param>
		public static Vector2 RandomVect2(Vector2 v) {
			return new Vector2 (Random.value * v.x, Random.value * v.y);
		}

		/// <summary>
		/// Randoms the vect3.
		/// </summary>
		/// <returns>The vect3.</returns>
		/// <param name="v">V.</param>
		public static Vector3 RandomVect3(Vector3 v) {
			return new Vector3 (Random.value * v.x, Random.value * v.y, Random.value * v.z);
		}

		/// <summary>
		/// Components the product2.
		/// </summary>
		/// <returns>The product2.</returns>
		/// <param name="v1">V1.</param>
		/// <param name="v2">V2.</param>
		public static Vector2 ComponentProduct2(Vector2 v1, Vector2 v2) {
			return new Vector2(v1.x * v2.x, v1.y * v2.y);
		}

		/// <summary>
		/// Components the product3.
		/// </summary>
		/// <returns>The product3.</returns>
		/// <param name="v1">V1.</param>
		/// <param name="v2">V2.</param>
		public static Vector3 ComponentProduct3(Vector3 v1, Vector3 v2) {
			return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
		}

		/// <summary>
		/// Maxs the component2.
		/// </summary>
		/// <returns>The component2.</returns>
		/// <param name="v">V.</param>
		public static float MaxComponent2(Vector2 v) {
			return (v.x > v.y) ? v.x : v.y;
		}

		/// <summary>
		/// Maxs the component3.
		/// </summary>
		/// <returns>The component3.</returns>
		/// <param name="v">V.</param>
		public static float MaxComponent3(Vector3 v) {
			if(v.x > v.y)
				return (v.z > v.y) ? v.z : v.y;
			else
				return (v.z > v.x) ? v.z : v.x;
		}

		/// <summary>
		/// Minimums the component2.
		/// </summary>
		/// <returns>The component2.</returns>
		/// <param name="v">V.</param>
		public static float MinComponent2(Vector2 v) {
			return (v.x < v.y) ? v.x : v.y;
		}

		/// <summary>
		/// Minimums the component3.
		/// </summary>
		/// <returns>The component3.</returns>
		/// <param name="v">V.</param>
		public static float MinComponent3(Vector3 v) {
			if(v.x < v.y)
				return (v.z < v.y) ? v.z : v.y;
			else
				return (v.z < v.x) ? v.z : v.x;
		}
		/// <summary>
		/// Berziers the curve vector lerp.
		/// </summary>
		/// <returns>The curve vector lerp.</returns>
		/// <param name="start">Start.</param>
		/// <param name="end">End.</param>
		/// <param name="c1">C1.</param>
		/// <param name="c2">C2.</param>
		/// <param name="t">T.</param>
		public static Vector3 BerzierCurveVectorLerp(Vector3 start, Vector3 end, Vector3 c1, Vector3 c2, float t) {
			float u, uu, uuu, tt, ttt;
			Vector3 p, p0 = start, p1 = c1, p2 = c2, p3 = end;
			u = 1 - t;
			uu = u*u;
			uuu = uu * u;
			tt = t * t;
			ttt = tt * t;
			
			p = uuu * p0; //first term
			p += 3 * uu * t * p1; //second term
			p += 3 * u * tt * p2; //third term
			p += ttt * p3; //fourth term

			return p;
		}

		public static T FindClosest<T>(Vector3 position) where T : Component {
			T returnValue = default(T);
			T[] objects = GameObject.FindObjectsOfType<T> ();
			float minDist = float.MaxValue;
			for (int i = 0; i < objects.Length; i++) {
				float dist = (objects[i].transform.position - position).magnitude;
				if(dist < minDist) {
					returnValue = objects[i];
					minDist = dist;
				}
			}
			return returnValue;
		}
	}
}