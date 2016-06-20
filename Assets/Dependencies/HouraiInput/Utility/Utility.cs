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

namespace HouraiTeahouse.HouraiInput {
    public static class Utility {
        static readonly Vector2[] circleVertexList;

        static Utility() {
            const int size = 20;
            circleVertexList = new Vector2[size];
            for (var i = 0; i < circleVertexList.Length; i++) {
                float angle = i * Mathf.PI * 2 / size;
                circleVertexList[i] = new Vector2(Mathf.Cos(angle),
                    Mathf.Sin(angle));
            }
        }

        public static void DrawCircleGizmo(Vector2 center, float radius) {
            Vector2 p = circleVertexList[0] * radius + center;
            int c = circleVertexList.Length;
            for (var i = 1; i < c; i++)
                Gizmos.DrawLine(p, p = circleVertexList[i] * radius + center);
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

        public static bool GameObjectIsCulledOnCurrentCamera(
            GameObject gameObject) {
            return !gameObject.LayerCheck(Camera.current.cullingMask);
        }


        public static float ApplyDeadZone(float value,
                                          float lowerDeadZone,
                                          float upperDeadZone) {
            return Mathf.InverseLerp(lowerDeadZone,
                upperDeadZone,
                Mathf.Abs(value)) * Mathf.Sign(value);
        }

        public static Vector2 ApplyCircularDeadZone(Vector2 axisVector,
                                                    float lowerDeadZone,
                                                    float upperDeadZone) {
            float magnitude = Mathf.InverseLerp(lowerDeadZone,
                upperDeadZone,
                axisVector.magnitude);
            return axisVector.normalized * magnitude;
        }
    }
}