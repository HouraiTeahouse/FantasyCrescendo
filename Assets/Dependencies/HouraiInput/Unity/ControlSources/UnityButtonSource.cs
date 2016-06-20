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
    public class UnityButtonSource : InputSource {
        static string[,] _buttonQueries;
        readonly int _buttonId;

        public UnityButtonSource(int buttonId) {
            _buttonId = buttonId;
            SetupButtonQueries();
        }

        public float GetValue(InputDevice inputDevice) {
            return GetState(inputDevice) ? 1.0f : 0.0f;
        }

        public bool GetState(InputDevice inputDevice) {
            var unityInputDevice = inputDevice as UnityInputDevice;
            if (unityInputDevice == null)
                return false;
            int joystickId = unityInputDevice.JoystickId;
            string buttonKey = GetButtonKey(joystickId, _buttonId);
            return Input.GetKey(buttonKey);
        }

        static void SetupButtonQueries() {
            if (_buttonQueries != null)
                return;
            _buttonQueries =
                new string[UnityInputDevice.MaxDevices,
                    UnityInputDevice.MaxButtons];

            for (var joystickId = 1;
                 joystickId <= UnityInputDevice.MaxDevices;
                 joystickId++)
                for (var buttonId = 0;
                     buttonId < UnityInputDevice.MaxButtons;
                     buttonId++)
                    _buttonQueries[joystickId - 1, buttonId] =
                        "joystick {0} button {1}".With(joystickId, buttonId);
        }

        static string GetButtonKey(int joystickId, int buttonId) {
            return _buttonQueries[joystickId - 1, buttonId];
        }
    }
}