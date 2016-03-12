using UnityEngine;

namespace HouraiTeahouse {
    /// <summary>
    /// A NumberText element that changes the color the Text based on a the current number value and a defined Gradient.
    /// </summary>
    public class GradientNumberText : NumberText {
        [SerializeField] private Gradient _gradient;

        [SerializeField] private float _start;

        [SerializeField] private float _end;

        /// <summary>
        /// The Color gradient used to determine the color of the text.
        /// </summary>
        public Gradient Gradient {
            get { return _gradient; }
            set { _gradient = value; }
        }

        /// <summary>
        /// The start value of the Gradient.
        /// When Number is less than this, the color that is used is sampled at the lower end of the gradient.
        /// </summary>
        public float Start {
            get { return _start; }
            set { _start = value; }
        }

        /// <summary>
        /// The end value of of the Gradient.
        /// When Number is greater than this, the color tha tis used is sampled from the upper end of the gradient.
        /// </summary>
        public float End {
            get { return _end; }
            set { _end = value; }
        }

        protected override void UpdateText() {
            base.UpdateText();

            if (_start > _end) {
                float temp = _start;
                _start = _end;
                _end = temp;
            }

            float point = _start == _end ? 0f : Mathf.Clamp01((Number - _start) / (_end - _start));

            Text.color = _gradient.Evaluate(point);
        }
    }
}