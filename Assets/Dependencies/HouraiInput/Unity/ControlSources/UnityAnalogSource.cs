using UnityEngine;

namespace HouraiTeahouse.HouraiInput {
    public class UnityAnalogSource : InputSource {
        private readonly int _analogId;
        private static string[,] _analogQueries;

        public UnityAnalogSource(int analogId) {
            _analogId = analogId;
            SetupAnalogQueries();
        }

        public float GetValue(InputDevice inputDevice) {
            var unityInputDevice = inputDevice as UnityInputDevice;
            if (unityInputDevice == null) return 0f;
            int joystickId = unityInputDevice.JoystickId;
            string analogKey = GetAnalogKey(joystickId, _analogId);
            return Input.GetAxisRaw(analogKey);
        }

        public bool GetState(InputDevice inputDevice) {
            return !Mathf.Approximately(GetValue(inputDevice), 0.0f);
        }

        private static void SetupAnalogQueries() {
            if (_analogQueries != null) return;
            _analogQueries = new string[UnityInputDevice.MaxDevices, UnityInputDevice.MaxAnalogs];

            for (var joystickId = 1; joystickId <= UnityInputDevice.MaxDevices; joystickId++) 
                for (var analogId = 0; analogId < UnityInputDevice.MaxAnalogs; analogId++) 
                    _analogQueries[joystickId - 1, analogId] = "joystick " + joystickId + " analog " + analogId;
        }

        private static string GetAnalogKey(int joystickId, int analogId) {
            return _analogQueries[joystickId - 1, analogId];
        }
    }
}
