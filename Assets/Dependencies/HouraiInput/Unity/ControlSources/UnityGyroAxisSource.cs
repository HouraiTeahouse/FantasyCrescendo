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
    // This is kind of "beta"... while it works on iOS, gyro controls are
    // inconsistent and are usually fine tuned to the games that use them
    // which is somewhat beyond the scope of this project. But, if you 
    // are curious how to go about it, here you go.
    public class UnityGyroAxisSource : InputSource {
        public enum GyroAxis {
            X = 0,
            Y = 1,
        }

        static Quaternion _zeroAttitude;

        readonly GyroAxis _axis;

        public UnityGyroAxisSource(GyroAxis axis) {
            _axis = axis;
            Calibrate();
        }

        public float GetValue(InputDevice inputDevice) {
            return GetAxis()[(int) _axis];
        }

        public bool GetState(InputDevice inputDevice) {
            return !Mathf.Approximately(GetValue(inputDevice), 0.0f);
        }

        static Quaternion GetAttitude() {
            return Quaternion.Inverse(_zeroAttitude) * Input.gyro.attitude;
        }

        static Vector3 GetAxis() {
            Vector3 gv = GetAttitude() * Vector3.forward;
            float gx = ApplyDeadZone(Mathf.Clamp(gv.x, -1.0f, 1.0f));
            float gy = ApplyDeadZone(Mathf.Clamp(gv.y, -1.0f, 1.0f));
            return new Vector3(gx, gy);
        }

        static float ApplyDeadZone(float value) {
            return Mathf.InverseLerp(0.05f, 1.0f, Mathf.Abs(value))
                * Mathf.Sign(value);
        }

        public static void Calibrate() { _zeroAttitude = Input.gyro.attitude; }
    }
}