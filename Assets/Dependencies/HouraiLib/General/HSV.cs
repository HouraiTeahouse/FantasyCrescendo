using System;
using UnityEngine;

namespace HouraiTeahouse {

    /// <summary>
    /// HSV (Hue/Saturation/Value) color struct.
    /// </summary>
    [Serializable]
    public struct HSV {

        /// <summary>
        /// The hue of the color. Range: [0, 360)
        /// </summary>
        public float h;

        /// <summary>
        /// The saturation of the color. Range: [0, 1]
        /// </summary>
        public float s;

        /// <summary>
        /// The value of the color. Range: [0, 1]
        /// </summary>
        public float v;

        /// <summary>
        /// The alpha of the color. Range: [0, 1]
        /// </summary>
        public float a;

        /// <summary>
        /// Initializes an instance of HSV.
        /// </summary>
        /// <param name="h">the hue of the color</param>
        /// <param name="s">the saturation of the color</param>
        /// <param name="v">the value of the color</param>
        /// <param name="a">the alpha of  the color</param>
        public HSV(float h, float s, float v, float a) {
            this.h = h;
            this.s = s;
            this.v = v;
            this.a = a;
        }

        /// <summary>
        /// Initializes an instance of HSV.
        /// </summary>
        /// <param name="h">the hue of the color</param>
        /// <param name="s">the saturation of the color</param>
        /// <param name="v">the value of the color</param>
        public HSV(float h, float s, float v) {
            this.h = h;
            this.s = s;
            this.v = v;
            this.a = 1f;
        }

        /// <summary>
        /// Initializes an instance of HSV from a RGBA color.
        /// </summary>
        /// <param name="col">the source Color</param>
        public HSV(Color col) : this(0, 0, 0, col.a) {
            float r = col.r;
            float g = col.g;
            float b = col.b;

            float max = Mathf.Max(r, Mathf.Max(g, b));

            if (max <= 0)
                return;

            float min = Mathf.Min(r, Mathf.Min(g, b));
            float dif = max - min;

            if (max > min) {
                if (g == max) {
                    h = (b - r) / dif * 60f + 120f;
                }
                else if (b == max) {
                    h = (r - g) / dif * 60f + 240f;
                }
                else if (b > g) {
                    h = (g - b) / dif * 60f + 360f;
                }
                else {
                    h = (g - b) / dif * 60f;
                }
                if (h < 0) {
                    h = h + 360f;
                }
            }
            else {
                h = 0;
            }

            h *= 1f / 360f;
            s = (dif / max) * 1f;
            v = max;
        }

        /// <summary>
        /// Converts the HSVA representatio to a RGBA representation. 
        /// </summary>
        /// <returns>the RGBA color representation</returns>
        public Color ToColor() {
            float r = v;
            float g = v;
            float b = v;
            if (s != 0) {
                float max = v;
                float dif = v * s;
                float min = v - dif;

                float hue = h * 360f;

                if (hue < 60f) {
                    r = max;
                    g = hue * dif / 60f + min;
                    b = min;
                }
                else if (hue < 120f) {
                    r = -(hue - 120f) * dif / 60f + min;
                    g = max;
                    b = min;
                }
                else if (hue < 180f) {
                    r = min;
                    g = max;
                    b = (hue - 120f) * dif / 60f + min;
                }
                else if (hue < 240f) {
                    r = min;
                    g = -(hue - 240f) * dif / 60f + min;
                    b = max;
                }
                else if (hue < 300f) {
                    r = (hue - 240f) * dif / 60f + min;
                    g = min;
                    b = max;
                }
                else if (hue <= 360f) {
                    r = max;
                    g = min;
                    b = -(hue - 360f) * dif / 60 + min;
                }
                else {
                    r = 0;
                    g = 0;
                    b = 0;
                }
            }

            return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), a);
        }


        public override string ToString() {
            return string.Format("(H:{0}, S:{1}, V:{2}, A:{3}", h, s, v, a);
        }
    }
}
