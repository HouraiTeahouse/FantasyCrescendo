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

namespace HouraiTeahouse.SmashBrew {
    public class SmashBrewKeyboardProfile : UnityInputDeviceProfile {
        public SmashBrewKeyboardProfile() {
            Name = "Smash Brew Single Player Keybard Profile";
            Meta = "Smash Brew Single Player Keybard Profile";

            SupportedPlatforms = new[] {"Windows", "OS X", "Linux"};

            ButtonMappings = new[] {
                new InputMapping {
                    Handle = "Attack",
                    Target = InputTarget.Action1,
                    Source = KeyCodeButton(KeyCode.Z)
                },
                new InputMapping {
                    Handle = "Special",
                    Target = InputTarget.Action2,
                    Source = KeyCodeButton(KeyCode.X)
                },
                new InputMapping {
                    Handle = "Jump",
                    Target = InputTarget.Action3,
                    Source = KeyCodeButton(KeyCode.LeftShift)
                },
                new InputMapping {
                    Handle = "Start",
                    Target = InputTarget.Start,
                    Source = KeyCodeButton(KeyCode.Return)
                },
            };

            AnalogMappings = new[] {
                new InputMapping {
                    Handle = "Left Horizontal",
                    Target = InputTarget.LeftStickX,
                    Source = KeyCodeAxis(KeyCode.LeftArrow, KeyCode.RightArrow)
                },
                new InputMapping {
                    Handle = "Left Vertical",
                    Target = InputTarget.LeftStickY,
                    Source = KeyCodeAxis(KeyCode.DownArrow, KeyCode.UpArrow)
                },
                new InputMapping {
                    Handle = "Right Horizontal",
                    Target = InputTarget.RightStickX,
                    Source = KeyCodeAxis(KeyCode.A, KeyCode.D)
                },
                new InputMapping {
                    Handle = "Right Vertical",
                    Target = InputTarget.RightStickY,
                    Source = KeyCodeAxis(KeyCode.S, KeyCode.W)
                }
            };
        }
    }
}