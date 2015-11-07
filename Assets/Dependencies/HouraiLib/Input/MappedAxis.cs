using System;
using UnityEngine;
using System.Collections;

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

        public void Update() { }

        public float GetValue() {
            return Map.Evaluate(BaseAxis.GetValue());
        }

    }

}
