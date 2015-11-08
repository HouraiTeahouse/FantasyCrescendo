using UnityEngine;
using System.Collections;

namespace Hourai {

    public static class InputAxisExtensions {

        public static IInputAxis Smooth(this IInputAxis axis, float power = 2f) {
            return new SmoothedInput(axis, power);
        }

        public static IInputAxis Map(this IInputAxis axis, AnimationCurve map) {
            return new MappedAxis(axis, map);
        }

    }

}
