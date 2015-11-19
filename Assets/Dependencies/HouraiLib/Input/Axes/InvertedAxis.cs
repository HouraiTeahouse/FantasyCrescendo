using System;

namespace Hourai {

    public class InvertedAxis : IInputAxis {

        private IInputAxis _baseAxis;

        public InvertedAxis(IInputAxis baseAxis) {
            if(baseAxis == null)
                throw new ArgumentNullException("baseAxis");
            baseAxis = _baseAxis;
        }

        public float GetAxisValue() {
            return -_baseAxis.GetAxisValue();
        }

    }

}