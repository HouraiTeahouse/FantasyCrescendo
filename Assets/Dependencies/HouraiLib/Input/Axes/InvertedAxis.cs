using System;

namespace Hourai {

    public class InvertedAxis : MetaAxis {

        public InvertedAxis(IInputAxis baseAxis) : base(baseAxis) {}

        public override float GetAxisValue() {
            return -BaseAxis.GetAxisValue();
        }

    }

}