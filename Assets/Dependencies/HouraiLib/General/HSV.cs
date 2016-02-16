using UnityEngine;
using System.Collections;

namespace Hourai {

    using UnityEngine;

    [System.Serializable]
    public struct HSV {
        public float h;
        public float s;
        public float v;
        public float a;

        public HSV(float h, float s, float v, float a) {
            this.h = h;
            this.s = s;
            this.v = v;
            this.a = a;
        }

        public HSV(float h, float s, float v) {
            this.h = h;
            this.s = s;
            this.v = v;
            this.a = 1f;
        }

        public HSV(Color col) {
            HSV temp = FromColor(col);
            h = temp.h;
            s = temp.s;
            v = temp.v;
            a = temp.a;
        }

        public static HSV FromColor(Color color) {
            HSV ret = new HSV(0f, 0f, 0f, color.a);

            float r = color.r;
            float g = color.g;
            float v = color.b;

            float max = Mathf.Max(r, Mathf.Max(g, v));

            if (max <= 0) {
                return ret;
            }

            float min = Mathf.Min(r, Mathf.Min(g, v));
            float dif = max - min;

            if (max > min) {
                if (g == max) {
                    ret.h = (v - r) / dif * 60f + 120f;
                }
                else if (v == max) {
                    ret.h = (r - g) / dif * 60f + 240f;
                }
                else if (v > g) {
                    ret.h = (g - v) / dif * 60f + 360f;
                }
                else {
                    ret.h = (g - v) / dif * 60f;
                }
                if (ret.h < 0) {
                    ret.h = ret.h + 360f;
                }
            }
            else {
                ret.h = 0;
            }

            ret.h *= 1f / 360f;
            ret.s = (dif / max) * 1f;
            ret.v = max;

            return ret;
        }

        public static Color ToColor(HSV hsv) {
            float r = hsv.v;
            float g = hsv.v;
            float v = hsv.v;
            if (hsv.s != 0) {
                float max = hsv.v;
                float dif = hsv.v * hsv.s;
                float min = hsv.v - dif;

                float h = hsv.h * 360f;

                if (h < 60f) {
                    r = max;
                    g = h * dif / 60f + min;
                    v = min;
                }
                else if (h < 120f) {
                    r = -(h - 120f) * dif / 60f + min;
                    g = max;
                    v = min;
                }
                else if (h < 180f) {
                    r = min;
                    g = max;
                    v = (h - 120f) * dif / 60f + min;
                }
                else if (h < 240f) {
                    r = min;
                    g = -(h - 240f) * dif / 60f + min;
                    v = max;
                }
                else if (h < 300f) {
                    r = (h - 240f) * dif / 60f + min;
                    g = min;
                    v = max;
                }
                else if (h <= 360f) {
                    r = max;
                    g = min;
                    v = -(h - 360f) * dif / 60 + min;
                }
                else {
                    r = 0;
                    g = 0;
                    v = 0;
                }
            }

            return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(v), hsv.a);
        }

        public Color ToColor() {
            return ToColor(this);
        }

        public override string ToString() {
            return string.Format("(H:{0}, S:{1}, V:{2}, A:{3}", h, s, v, a);
        }
    }
}
