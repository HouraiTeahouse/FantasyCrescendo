using System;
using UnityEngine;

namespace Hourai {

    public class MappedAxis : IInputAxis {

        public readonly IInputAxis BaseAxis;
        public AnimationCurve Map { get; set; }

        public MappedAxis(IInputAxis baseAxis, AnimationCurve map) {
            if (baseAxis == null)
                throw new ArgumentNullException("baseAxis");
            if (map == null)
                throw new ArgumentNullException("map");
            BaseAxis = baseAxis;
            Map = map;
        }

        public float GetAxisValue() {
            return Map.Evaluate(BaseAxis.GetAxisValue());
        }

    }

}
