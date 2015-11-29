using System;

namespace Hourai {

    public class ButtonAxis : MetaAxis, IInputButton {

        private float _threshold;

        public float Threshold {
            get { return _threshold; }
            set {
                _threshold = Math.Abs(value);
                if (_threshold > 1f)
                    _threshold = 1f;
            }
        }

        public ButtonAxis(IInputAxis baseAxis, float threshold = 0.5f) : base(baseAxis) {
            Threshold = threshold;
        }

        public override float GetAxisValue() {
            return (Math.Abs(BaseAxis.GetAxisValue()) > Threshold) ? 1f : 0f;
        }

        public bool GetButtonValue() {
            return Math.Abs(BaseAxis.GetAxisValue()) > Threshold;
        }

    }

}