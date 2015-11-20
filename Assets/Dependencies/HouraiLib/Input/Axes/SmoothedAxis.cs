using System;
using UnityEngine;
using System.Collections;

namespace Hourai {

    public class SmoothedInput : MetaAxis {

        public float Power { get; set; }

        public SmoothedInput(IInputAxis baseAxis, float pow = 2) : base(baseAxis) {
            Power = pow;
        }

        public override float GetAxisValue() {
            return Mathf.Pow(BaseAxis.GetAxisValue(), Power);
        }

    }

}

