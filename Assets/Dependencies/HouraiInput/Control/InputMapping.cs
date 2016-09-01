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

namespace HouraiTeahouse.HouraiInput {

    public class InputMapping {

        public class Range {
            public static Range Complete = new Range {
                Minimum = -1.0f,
                Maximum = 1.0f
            };

            public static Range Positive = new Range {
                Minimum = 0.0f,
                Maximum = 1.0f
            };

            public static Range Negative = new Range {
                Minimum = -1.0f,
                Maximum = 0.0f
            };

            public float Maximum;

            public float Minimum;
        }

        string _handle;

        // This is primarily to fix a bug with the wired Xbox controller on Mac.
        public bool IgnoreInitialZeroValue;

        // Invert the final mapped value.
        public bool Invert;

        // Raw inputs won't be processed except for scaling (mice and trackpads).
        public bool Raw;

        // Analog values will be multiplied by this number before processing.
        public float Scale = 1.0f;


        public InputSource Source;

        public Range SourceRange = Range.Complete;
        public InputTarget Target;
        public Range TargetRange = Range.Complete;

        public string Handle {
            get { return string.IsNullOrEmpty(_handle) ? Target.ToString() : _handle; }
            set { _handle = value; }
        }

        bool IsYAxis {
            get {
                return Target == InputTarget.LeftStickY
                    || Target == InputTarget.RightStickY;
            }
        }

        public float MapValue(float value) {
            float targetValue;

            if (Raw)
                targetValue = value * Scale;
            else {
                // Scale value and clamp to a legal range.
                value = Mathf.Clamp(value * Scale, -1.0f, 1.0f);

                // Values outside of source range are invalid and return zero.
                if (value < SourceRange.Minimum || value > SourceRange.Maximum) {
                    return 0.0f;
                }

                // Remap from source range to target range.
                float sourceValue = Mathf.InverseLerp(SourceRange.Minimum,
                    SourceRange.Maximum,
                    value);
                targetValue = Mathf.Lerp(TargetRange.Minimum,
                    TargetRange.Maximum,
                    sourceValue);
            }

            if (Invert ^ (IsYAxis && HInput.InvertYAxis)) {
                targetValue = -targetValue;
            }

            return targetValue;
        }
    }
}
