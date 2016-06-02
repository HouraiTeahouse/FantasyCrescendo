using UnityEngine;

namespace HouraiTeahouse {
    
    public static class ColorExtensions {
        /// <summary>
        /// Moves a color towards another color.
        /// </summary>
        /// <param name="color0">the source color</param>
        /// <param name="color1">the target color</param>
        /// <param name="maxDelta">the biggest change color</param>
        /// <returns>the alternate color</returns>
        public static Color MoveTowards(this Color color0, Color color1, float maxDelta) {
            return Vector4.MoveTowards(color0, color1, maxDelta);
        }
    }

}

