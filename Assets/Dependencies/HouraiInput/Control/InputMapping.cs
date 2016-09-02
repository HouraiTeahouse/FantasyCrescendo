using UnityEngine;

namespace HouraiTeahouse.HouraiInput {

    public class InputMapping {

        public static readonly Range Complete = new Range(-1, 1);
        public static readonly Range Positive = new Range(0, 1);
        public static readonly Range Negative = new Range(-1, 0);

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

        public Range SourceRange = Complete;
        public InputTarget Target;
        public Range TargetRange = Complete;

        public string Handle {
            get { return string.IsNullOrEmpty(_handle) ? Target.ToString() : _handle; }
            set { _handle = value; }
        }

        bool IsYAxis {
            get { return Target == InputTarget.LeftStickY || Target == InputTarget.RightStickY; }
        }

        public float MapValue(float value) {
            float targetValue;

            if (Raw)
                targetValue = value * Scale;
            else {
                // Scale value and clamp to a legal range.
                value = Mathf.Clamp(value * Scale, -1.0f, 1.0f);

                // Values outside of source range are invalid and return zero.
                if (value < SourceRange.Min || value > SourceRange.Max) {
                    return 0.0f;
                }

                // Remap from source range to target range.
                float sourceValue = SourceRange.InverseLerp(value);
                targetValue = TargetRange.Lerp(sourceValue);
            }

            if (Invert ^ (IsYAxis && HInput.InvertYAxis)) {
                targetValue = -targetValue;
            }

            return targetValue;
        }

    }

}