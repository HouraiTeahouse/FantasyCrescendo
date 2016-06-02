using UnityEngine;

namespace HouraiTeahouse.HouraiInput {
    public static class Utility {

        private static readonly Vector2[] circleVertexList;

        static Utility() {
            const int size = 20;
            circleVertexList = new Vector2[size];
            for (var i = 0; i < circleVertexList.Length; i++) {
                float angle = i * Mathf.PI * 2 / size;
                circleVertexList[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)); 
            }
        }

        public static void DrawCircleGizmo(Vector2 center, float radius) {
            Vector2 p = (circleVertexList[0] * radius) + center;
            int c = circleVertexList.Length;
            for (var i = 1; i < c; i++)
                Gizmos.DrawLine(p, p = (circleVertexList[i] * radius) + center);
        }

        public static void DrawOvalGizmo(Vector2 center, Vector2 size) {
            Vector2 r = size / 2.0f;
            Vector2 p = circleVertexList[0].Mult(r) + center;
            int c = circleVertexList.Length;
            for (var i = 1; i < c; i++)
                Gizmos.DrawLine(p, p = circleVertexList[i].Mult(r) + center);
        }

        public static void DrawRectGizmo(Rect rect) {
            Vector2 p0 = rect.min;
            var p1 = new Vector3(rect.xMax, rect.yMin);
            Vector2 p2 = rect.max;
            var p3 = new Vector3(rect.xMin, rect.yMax);
            Gizmos.DrawLine(p0, p1);
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p0);
        }

        public static void DrawRectGizmo(Vector2 center, Vector2 size) {
            DrawRectGizmo(new Rect(center - 0.5f * size, size));
        }

        public static bool GameObjectIsCulledOnCurrentCamera(GameObject gameObject) {
            return !gameObject.LayerCheck(Camera.current.cullingMask);
        }


        public static float ApplyDeadZone(float value, float lowerDeadZone, float upperDeadZone) {
            return Mathf.InverseLerp(lowerDeadZone, upperDeadZone, Mathf.Abs(value)) * Mathf.Sign(value);
        }

        public static Vector2 ApplyCircularDeadZone(Vector2 axisVector, float lowerDeadZone, float upperDeadZone) {
            float magnitude = Mathf.InverseLerp(lowerDeadZone, upperDeadZone, axisVector.magnitude);
            return axisVector.normalized * magnitude;
        }
    }
}
