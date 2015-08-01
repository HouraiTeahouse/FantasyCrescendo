using UnityEngine;

namespace Crescendo.API {


    public static class VectorExtensions {

        public static Vector2 MaxComponent2(this Vector2 v) {
            if (v.x > v.y)
                return new Vector2(v.x, 0f);
            return new Vector2(0f, v.y);
        }

        public static Vector3 MaxComponent3(this Vector3 v) {
            if (v.x > v.y && v.x > v.z)
               return new Vector3(v.x, 0f, 0f);
            if(v.y > v.z)
                return new Vector3(0f, v.y, 0f);
            return new Vector3(0f, 0f, v.z);
        }

        public static float Max2(this Vector2 v) {
            return Mathf.Max(v.x, v.y);
        }

        public static float Max3(this Vector3 v) {
            return Mathf.Max(v.x, v.y, v.z);
        }


        public static Vector2 MinComponent2(this Vector2 v) {
            if (v.x > v.y)
                return new Vector2(v.x, 0f);
            return new Vector2(0f, v.y);
        }

        public static Vector3 MinComponent3(this Vector3 v) {
            if (v.x < v.y && v.x < v.z)
                return new Vector3(v.x, 0f, 0f);
            if (v.y < v.z)
                return new Vector3(0f, v.y, 0f);
            return new Vector3(0f, 0f, v.z);
        }

        public static float Min2(this Vector2 v) {
            return Mathf.Min(v.x, v.y);
        }

        public static float Min3(this Vector3 v) {
            return Mathf.Min(v.x, v.y, v.z);
        }

    }

}

