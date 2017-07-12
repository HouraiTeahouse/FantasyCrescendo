using System;
using System.Collections.Generic;
using System.Linq;
using HouraiTeahouse.HouraiInput;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    [Serializable]
    public sealed class PlayerControlMapping {

        static readonly Func<InputControl, float> Val = c => c.Value;
        static readonly Func<InputControl, bool> Pressed = c => c.State;

        public InputTarget[] AltStickHorizontalTargets = {InputTarget.RightStickX};
        public InputTarget[] AltStickVerticalTargets = {InputTarget.RightStickY};
        public InputTarget[] AttackTargets = {InputTarget.Action1};
        public InputTarget[] JumpTargets = {InputTarget.Action3, InputTarget.Action4};
        public InputTarget[] ShieldTargets = {InputTarget.LeftTrigger, InputTarget.RightTrigger};
        public InputTarget[] SpecialTargets = {InputTarget.Action2};
        public InputTarget[] StickHorizontalTargets = {InputTarget.LeftStickX};
        public InputTarget[] StickVerticalTargets = {InputTarget.LeftStickY};

        static bool Check(InputDevice device, IEnumerable<InputTarget> targets) {
            return device != null && device.GetControls(targets).Any(Pressed);
        }

        static Vector2 Direction(InputDevice device,
                                 IEnumerable<InputTarget> targetsX,
                                 IEnumerable<InputTarget> targetsY) {
            if (device == null)
                return new Vector2();
            return new Vector2 {
                x = device.GetControls(targetsX).Average(Val),
                y = device.GetControls(targetsY).Average(Val)
            };
        }

        public bool Attack(InputDevice device) { return Check(device, AttackTargets); }

        public bool Special(InputDevice device) { return Check(device, SpecialTargets); }

        public bool Shield(InputDevice device) { return Check(device, ShieldTargets); }

        public bool Jump(InputDevice device) { return Check(device, JumpTargets); }

        public Vector2 Stick(InputDevice device) {
            return Direction(device, StickHorizontalTargets, StickVerticalTargets);
        }

        public Vector2 AltStick(InputDevice device) {
            return Direction(device, AltStickHorizontalTargets, AltStickVerticalTargets);
        }

    }

}