using System;
using UnityEngine;

namespace Hourai {

    public abstract class MetaAxis : IInputAxis {

        protected internal IInputAxis BaseAxis { get; private set; }

        protected MetaAxis(IInputAxis baseAxis) {
            if(baseAxis == null)
                throw new ArgumentNullException("baseAxis");
            BaseAxis = baseAxis;
        }

        public abstract float GetAxisValue();

    }

    public class SmoothedAxis : MetaAxis {

        public float Power { get; set; }

        public SmoothedAxis(IInputAxis baseAxis, float pow = 2) : base(baseAxis) {
            Power = pow;
        }

        public override float GetAxisValue() {
            return Mathf.Pow(BaseAxis.GetAxisValue(), Power);
        }

    }

    public class MappedAxis : MetaAxis {

        public AnimationCurve Map { get; set; }

        public MappedAxis(IInputAxis baseAxis, AnimationCurve map) : base(baseAxis) {
            Map = map;
        }

        public override float GetAxisValue() {
            return Map.Evaluate(BaseAxis.GetAxisValue());
        }

    }

    public class InvertedAxis : MetaAxis {

        public InvertedAxis(IInputAxis baseAxis) : base(baseAxis) { }

        public override float GetAxisValue() {
            return -BaseAxis.GetAxisValue();
        }

    }

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