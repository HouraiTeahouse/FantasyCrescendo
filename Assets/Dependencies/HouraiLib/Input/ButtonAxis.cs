using System;

namespace Hourai {

    public class ButtonAxis : IInputAxis, IInputButton {

        private readonly IInputAxis _baseAxis;
        private float _threshold;

        public float Threshold {
            get { return _threshold; }
            set {
                _threshold = Math.Abs(value);
                if (_threshold > 1f)
                    _threshold = 1f;
            }
        }

        public ButtonAxis(IInputAxis baseAxis, float threshold = 0.5f) {
            if(baseAxis == null)
                throw new ArgumentNullException("baseAxis");
            _baseAxis = baseAxis;
            Threshold = threshold;
        }

        public float GetAxisValue() {
            return (Math.Abs(_baseAxis.GetAxisValue()) > Threshold) ? 1f : 0f;
        }

        public bool GetButtonValue() {
            return Math.Abs(_baseAxis.GetAxisValue()) > Threshold;
        }

    }

}