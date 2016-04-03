using UnityEngine;

namespace HouraiTeahouse.HouraiInput {
    public class UnityButtonSource : InputSource {
        private readonly int _buttonId;
        private static string[,] _buttonQueries;

        public UnityButtonSource(int buttonId) {
            _buttonId = buttonId;
            SetupButtonQueries();
        }

        public float GetValue(InputDevice inputDevice) {
            return GetState(inputDevice) ? 1.0f : 0.0f;
        }

        public bool GetState(InputDevice inputDevice) {
            var unityInputDevice = inputDevice as UnityInputDevice;
            if (unityInputDevice == null) return false;
            int joystickId = unityInputDevice.JoystickId;
            string buttonKey = GetButtonKey(joystickId, _buttonId);
            return Input.GetKey(buttonKey);
        }

        private static void SetupButtonQueries() {
            if (_buttonQueries != null) return;
            _buttonQueries = new string[UnityInputDevice.MaxDevices, UnityInputDevice.MaxButtons];

            for (var joystickId = 1; joystickId <= UnityInputDevice.MaxDevices; joystickId++)
                for (var buttonId = 0; buttonId < UnityInputDevice.MaxButtons; buttonId++) 
                    _buttonQueries[joystickId - 1, buttonId] = "joystick " + joystickId + " button " + buttonId;
        }

        private static string GetButtonKey(int joystickId, int buttonId) {
            return _buttonQueries[joystickId - 1, buttonId];
        }
    }
}
