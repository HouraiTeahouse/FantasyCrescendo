using UnityEngine;

namespace HouraiTeahouse.HouraiInput {
    public class UnityKeyCodeAxisSource : InputSource {
        public KeyCode NegativeKeyCode { get; set; }
        public KeyCode PositiveKeyCode { get; set; }

        public UnityKeyCodeAxisSource(KeyCode negativeKeyCode, KeyCode positiveKeyCode) {
            NegativeKeyCode = negativeKeyCode;
            PositiveKeyCode = positiveKeyCode;
        }

        public float GetValue(InputDevice inputDevice) {
            var axisValue = 0;
            if (Input.GetKey(NegativeKeyCode))
                axisValue--;
            if (Input.GetKey(PositiveKeyCode))
                axisValue++;
            return axisValue;
        }

        public bool GetState(InputDevice inputDevice) {
            return !Mathf.Approximately(GetValue(inputDevice), 0.0f);
        }
    }
}
