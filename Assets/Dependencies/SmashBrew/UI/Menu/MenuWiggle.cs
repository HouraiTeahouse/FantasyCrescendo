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

using HouraiTeahouse.HouraiInput;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {
    public class MenuWiggle : MonoBehaviour {
        [SerializeField]
        InputTarget _horizontalAxis = InputTarget.RightStickX;

        [SerializeField]
        Vector2 _scale = new Vector2(30, 30);

        [SerializeField]
        InputTarget _verticalAxis = InputTarget.RightStickY;

        /// <summary> Unity Callback. Called once every frame. </summary>
        void Update() {
            Vector2 distortion = Vector2.zero;
            int count = HInput.Devices.Count;
            for (var i = 0; i < count; i++) {
                InputDevice device = HInput.Devices[i];
                if (device == null)
                    continue;
                float x = device[_verticalAxis];
                float y = device[_horizontalAxis];
                if (Mathf.Abs(distortion.x) < Mathf.Abs(x))
                    distortion.x = x;
                if (Mathf.Abs(distortion.y) < Mathf.Abs(y))
                    distortion.y = y;
            }
            distortion = Vector2.ClampMagnitude(distortion, 1f);
            distortion.x *= _scale.x;
            distortion.y *= _scale.y;
            transform.rotation = Quaternion.Euler(distortion);
        }
    }
}