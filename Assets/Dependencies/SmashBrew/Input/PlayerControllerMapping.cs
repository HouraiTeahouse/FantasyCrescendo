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

using System;
using System.Collections.Generic;
using System.Linq;
using HouraiTeahouse.HouraiInput;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    [Serializable]
    public sealed class PlayerControlMapping {
        static readonly Func<InputControl, float> val = c => c.Value;
        static readonly Func<InputControl, bool> pressed = c => c.WasPressed;

        public InputTarget[] AltStickHorizontalTargets = {
            InputTarget.RightStickX
        };

        public InputTarget[] AltStickVerticalTargets = {InputTarget.RightStickY};
        public InputTarget[] AttackTargets = {InputTarget.Action1};

        public InputTarget[] JumpTargets = {
            InputTarget.Action3,
            InputTarget.Action4
        };

        public InputTarget[] ShieldTargets = {
            InputTarget.LeftTrigger,
            InputTarget.RightTrigger
        };

        public InputTarget[] SpecialTargets = {InputTarget.Action2};

        public InputTarget[] StickHorizontalTargets = {InputTarget.LeftStickX};
        public InputTarget[] StickVerticalTargets = {InputTarget.LeftStickY};

        static bool Check(InputDevice device, IEnumerable<InputTarget> targets) {
            return device != null && device.GetControls(targets).Any(pressed);
        }

        static Vector2 Direction(InputDevice device,
                                 IEnumerable<InputTarget> targetsX,
                                 IEnumerable<InputTarget> targetsY) {
            var area = new Vector2();
            if (device == null)
                return area;
            area.x = device.GetControls(targetsX).Average(val);
            area.y = device.GetControls(targetsY).Average(val);
            return area;
        }

        public bool Attack(InputDevice device) {
            return Check(device, AttackTargets);
        }

        public bool Special(InputDevice device) {
            return Check(device, SpecialTargets);
        }

        public bool Shield(InputDevice device) {
            return Check(device, ShieldTargets);
        }

        public bool Jump(InputDevice device) {
            return Check(device, JumpTargets);
        }

        public Vector2 Stick(InputDevice device) {
            return Direction(device,
                StickHorizontalTargets,
                StickVerticalTargets);
        }

        public Vector2 AltStick(InputDevice device) {
            return Direction(device,
                AltStickHorizontalTargets,
                AltStickVerticalTargets);
        }
    }
}