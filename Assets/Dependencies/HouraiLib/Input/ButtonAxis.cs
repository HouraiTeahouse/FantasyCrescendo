using System;

namespace Hourai {

    public class ButtonAxis : IInputAxis {

        private readonly IInputAxis _baseAxis;
        private float _threshold;

        public float Threashold {
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
            Threashold = threshold;
        }

        public void Update() {}

        public float GetValue() {
            return (Math.Abs(_baseAxis.GetValue()) > Threashold) ? 1f : 0f;
        }

    }

}