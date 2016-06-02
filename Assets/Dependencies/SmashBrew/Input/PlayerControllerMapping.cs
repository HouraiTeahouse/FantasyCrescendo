using System;
using System.Collections.Generic;
using System.Linq;
using HouraiTeahouse.HouraiInput;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    [Serializable]
    public sealed class PlayerControlMapping {
        public InputTarget[] AttackTargets = {InputTarget.Action1};
        public InputTarget[] SpecialTargets = {InputTarget.Action2};
        public InputTarget[] ShieldTargets = {InputTarget.LeftTrigger, InputTarget.RightTrigger};
        public InputTarget[] JumpTargets = {InputTarget.Action3, InputTarget.Action4};
        public InputTarget[] StickHorizontalTargets = {InputTarget.LeftStickX};
        public InputTarget[] StickVerticalTargets = {InputTarget.LeftStickY};
        public InputTarget[] AltStickHorizontalTargets = {InputTarget.RightStickX};
        public InputTarget[] AltStickVerticalTargets = {InputTarget.RightStickY};

        static readonly Func<InputControl, float> val = c => c.Value;
        static readonly Func<InputControl, bool> pressed = c => c.WasPressed;

        static bool Check(InputDevice device, IEnumerable<InputTarget> targets) {
            return device != null && device.GetControls(targets).Any(pressed);
        }

        static Vector2 Direction(InputDevice device, IEnumerable<InputTarget> targetsX, IEnumerable<InputTarget> targetsY) {
            var area = new Vector2();
            if (device == null) return area;
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
            return Direction(device, StickHorizontalTargets, StickVerticalTargets);
        }

        public Vector2 AltStick(InputDevice device) {
            return Direction(device, AltStickHorizontalTargets, AltStickVerticalTargets);
        }
    }
}
