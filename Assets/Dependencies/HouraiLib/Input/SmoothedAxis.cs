using System;
using UnityEngine;
using System.Collections;

namespace Hourai {

    public class SmoothedInput : IInputAxis {

        public readonly IInputAxis BaseAxis;
        public float Power { get; set; }

        public SmoothedInput(IInputAxis baseAxis, float pow = 2) {
            if (baseAxis == null)
                throw new ArgumentNullException("baseAxis");
            BaseAxis = baseAxis;
            Power = pow;
        }

        public void Update() {}

        public float GetValue() {
            return Mathf.Pow(BaseAxis.GetValue(), Power);
        }

    }

}

