using UnityEngine;

namespace HouraiTeahouse.HouraiInput {
    public class UnityMouseButtonSource : InputSource {
        private readonly int _buttonId;

        public UnityMouseButtonSource(int buttonId) {
            _buttonId = buttonId;
        }

        public float GetValue(InputDevice inputDevice) {
            return GetState(inputDevice) ? 1.0f : 0.0f;
        }

        public bool GetState(InputDevice inputDevice) {
            return Input.GetMouseButton(_buttonId);
        }
    }
}
