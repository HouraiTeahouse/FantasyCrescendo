using UnityEngine;
using System.Collections;

namespace Hourai {

    public class AxisButton : MetaButton, IInputAxis {

        public AxisButton(IInputButton baseButton) : base(baseButton) {}

        public override bool GetButtonValue() {
            return BaseButton.GetButtonValue();
        }

        public float GetAxisValue() {
            return (GetButtonValue()) ? 1f : 0f;
        }

    }

}
