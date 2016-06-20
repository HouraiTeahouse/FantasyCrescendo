// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using UnityEngine;

namespace HouraiTeahouse {
    /// <summary> A NumberText element that changes the color the Text based on a the current number value and a defined
    /// Gradient. </summary>
    public class GradientNumberText : NumberText {
        [SerializeField]
        float _end;

        [SerializeField]
        Gradient _gradient;

        [SerializeField]
        float _start;

        /// <summary> The Color gradient used to determine the color of the text. </summary>
        public Gradient Gradient {
            get { return _gradient; }
            set { _gradient = value; }
        }

        /// <summary> The start value of the Gradient. When Number is less than this, the color that is used is sampled at the
        /// lower end of the gradient. </summary>
        public float Start {
            get { return _start; }
            set { _start = value; }
        }

        /// <summary> The end value of of the Gradient. When Number is greater than this, the color tha tis used is sampled from
        /// the upper end of the gradient. </summary>
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

            float point = _start == _end
                ? 0f
                : Mathf.Clamp01((Number - _start) / (_end - _start));

            Text.color = _gradient.Evaluate(point);
        }
    }
}