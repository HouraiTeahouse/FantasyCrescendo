using System;
using UnityEngine;

namespace HouraiTeahouse {
    /// <summary>
    /// HSV (Hue/Saturation/Value) color struct.
    /// </summary>
    [Serializable]
    public struct HSV {
        public static HSV white {
            get { return new HSV(Color.white); }
        }

        public static HSV black {
            get { return new HSV(Color.black); }
        }

        public static HSV clear {
            get { return new HSV(Color.clear); }
        }

        public static HSV red {
            get { return new HSV(Color.red); }
        }

        public static HSV green {
            get { return new HSV(Color.green); }
        }

        public static HSV blue {
            get { return new HSV(Color.blue); }
        }

        public static HSV cyan {
            get { return new HSV(Color.cyan); }
        }

        public static HSV magenta {
            get { return new HSV(Color.magenta); }
        }

        public static HSV yellow {
            get { return new HSV(Color.yellow); }
        }

        public static HSV grey {
            get { return new HSV(Color.grey); }
        }

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
        public HSV(Color col) {
            Color.RGBToHSV(col, out h, out s, out v);
            a = col.a;
        }

        /// <summary>
        /// Initializes an instance of HSV from another instance of HSV.
        /// </summary>
        /// <param name="src">the source HSV instance</param>
        public HSV(HSV src) : this(src.h, src.s, src.v, src.a) {
        }

        /// <summary>
        /// Initalizes an instance of HSV from a Vector3.
        /// </summary>
        /// <param name="src">the source vector</param>
        public HSV(Vector3 src) : this(src.x, src.y, src.z) {
        }

        /// <summary>
        /// Converts the HSVA representatio to a RGBA representation. 
        /// </summary>
        /// <returns>the RGBA color representation</returns>
        public Color ToColor() {
            Color col = Color.HSVToRGB(h, s, v);
            col.a = a;
            return col;
        }

        // implicit converter between color and HSV
        public static implicit operator Color(HSV hsv) {
            return hsv.ToColor();
        }

        // implicit converter between HSV and color
        public static implicit operator HSV(Color color) {
            return new HSV(color);
        }

        public override string ToString() {
            return string.Format("(H:{0}, S:{1}, V:{2}, A:{3})", h, s, v, a);
        }
    }
}