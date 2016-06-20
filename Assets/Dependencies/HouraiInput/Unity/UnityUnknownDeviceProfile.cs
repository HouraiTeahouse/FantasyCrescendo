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

namespace HouraiTeahouse.HouraiInput {
    public class UnityUnknownDeviceProfile : UnityInputDeviceProfile {
        public UnityUnknownDeviceProfile(string joystickName) {
            Name = "Unknown Device";
            if (joystickName != "") {
                Name += " (" + joystickName + ")";
            }

            Meta = "";
            Sensitivity = 1.0f;
            LowerDeadZone = 0.2f;

            SupportedPlatforms = null;
            JoystickNames = new[] {joystickName};

            AnalogMappings = new InputMapping[UnityInputDevice.MaxAnalogs];
            for (var i = 0; i < UnityInputDevice.MaxAnalogs; i++) {
                AnalogMappings[i] = new InputMapping {
                    Handle = "Analog " + i,
                    Source = Analog(i),
                    Target = InputTarget.Analog0 + i
                };
            }

            ButtonMappings = new InputMapping[UnityInputDevice.MaxButtons];
            for (var i = 0; i < UnityInputDevice.MaxButtons; i++) {
                ButtonMappings[i] = new InputMapping {
                    Handle = "Button " + i,
                    Source = Button(i),
                    Target = InputTarget.Button0 + i
                };
            }
        }

        public override bool IsKnown {
            get { return false; }
        }
    }
}