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
    public class UnityAnalogSource : InputSource {
        static string[,] _analogQueries;
        readonly int _analogId;

        public UnityAnalogSource(int analogId) {
            _analogId = analogId;
            SetupAnalogQueries();
        }

        public float GetValue(InputDevice inputDevice) {
            var unityInputDevice = inputDevice as UnityInputDevice;
            if (unityInputDevice == null)
                return 0f;
            int joystickId = unityInputDevice.JoystickId;
            string analogKey = GetAnalogKey(joystickId, _analogId);
            return Input.GetAxisRaw(analogKey);
        }

        public bool GetState(InputDevice inputDevice) {
            return !Mathf.Approximately(GetValue(inputDevice), 0.0f);
        }

        static void SetupAnalogQueries() {
            if (_analogQueries != null)
                return;
            _analogQueries =
                new string[UnityInputDevice.MaxDevices,
                    UnityInputDevice.MaxAnalogs];

            for (var joystickId = 1;
                 joystickId <= UnityInputDevice.MaxDevices;
                 joystickId++)
                for (var analogId = 0;
                     analogId < UnityInputDevice.MaxAnalogs;
                     analogId++)
                    _analogQueries[joystickId - 1, analogId] = "joystick "
                        + joystickId + " analog " + analogId;
        }

        static string GetAnalogKey(int joystickId, int analogId) {
            return _analogQueries[joystickId - 1, analogId];
        }
    }
}