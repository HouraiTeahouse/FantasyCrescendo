using System;
using UnityEngine;

namespace HouraiTeahouse.HouraiInput {

    public class InputDevice {

        public static readonly InputDevice Null = new InputDevice("NullInputDevice");

        static readonly int ControlCount = Enum.GetValues(typeof(InputTarget)).Length;

        internal int SortOrder = int.MaxValue;

        public InputDevice(string name) {
            Name = name;
            Meta = "";

            LastChangeTick = 0;

            int numInputControlTypes = ControlCount;
            Controls = new InputControl[numInputControlTypes];

            LeftStick = new TwoAxisInputControl();
            RightStick = new TwoAxisInputControl();
            DPad = new TwoAxisInputControl();
        }

        public string Name { get; protected set; }
        public string Meta { get; protected set; }

        public ulong LastChangeTick { get; protected set; }

        public InputControl[] Controls { get; protected set; }

        public TwoAxisInputControl LeftStick { get; protected set; }
        public TwoAxisInputControl RightStick { get; protected set; }
        public TwoAxisInputControl DPad { get; protected set; }

        public InputControl this[InputTarget target] {
            get { return GetControl(target); }
        }

        Vector2 DPadVector {
            get {
                float x = DPadLeft.State ? -DPadLeft.Value : DPadRight.Value;
                float t = DPadUp.State ? DPadUp.Value : -DPadDown.Value;
                float y = HInput.InvertYAxis ? -t : t;
                return new Vector2(x, y).normalized;
            }
        }

        public virtual bool IsSupportedOnThisPlatform {
            get { return true; }
        }

        public virtual bool IsKnown {
            get { return true; }
        }

        public bool MenuWasPressed {
            get {
                return GetControl(InputTarget.Back).WasPressed || GetControl(InputTarget.Start).WasPressed
                    || GetControl(InputTarget.Select).WasPressed || GetControl(InputTarget.System).WasPressed
                    || GetControl(InputTarget.Pause).WasPressed || GetControl(InputTarget.Menu).WasPressed;
            }
        }

        public InputControl AnyButton {
            get {
                for (var i = 0; i < Controls.Length; i++) {
                    InputControl control = Controls[i];
                    if (control != null && control.IsButton && control.IsPressed)
                        return control;
                }
                return InputControl.Null;
            }
        }

        public InputControl LeftStickX {
            get { return GetControl(InputTarget.LeftStickX); }
        }

        public InputControl LeftStickY {
            get { return GetControl(InputTarget.LeftStickY); }
        }

        public InputControl RightStickX {
            get { return GetControl(InputTarget.RightStickX); }
        }

        public InputControl RightStickY {
            get { return GetControl(InputTarget.RightStickY); }
        }

        public InputControl DPadUp {
            get { return GetControl(InputTarget.DPadUp); }
        }

        public InputControl DPadDown {
            get { return GetControl(InputTarget.DPadDown); }
        }

        public InputControl DPadLeft {
            get { return GetControl(InputTarget.DPadLeft); }
        }

        public InputControl DPadRight {
            get { return GetControl(InputTarget.DPadRight); }
        }

        public InputControl Action1 {
            get { return GetControl(InputTarget.Action1); }
        }

        public InputControl Action2 {
            get { return GetControl(InputTarget.Action2); }
        }

        public InputControl Action3 {
            get { return GetControl(InputTarget.Action3); }
        }

        public InputControl Action4 {
            get { return GetControl(InputTarget.Action4); }
        }

        public InputControl LeftTrigger {
            get { return GetControl(InputTarget.LeftTrigger); }
        }

        public InputControl RightTrigger {
            get { return GetControl(InputTarget.RightTrigger); }
        }

        public InputControl LeftBumper {
            get { return GetControl(InputTarget.LeftBumper); }
        }

        public InputControl RightBumper {
            get { return GetControl(InputTarget.RightBumper); }
        }

        public InputControl LeftStickButton {
            get { return GetControl(InputTarget.LeftStickButton); }
        }

        public InputControl RightStickButton {
            get { return GetControl(InputTarget.RightStickButton); }
        }

        public float DPadX {
            get { return DPad.X; }
        }

        public float DPadY {
            get { return DPad.Y; }
        }

        public TwoAxisInputControl Direction {
            get { return DPad.UpdateTick > LeftStick.UpdateTick ? DPad : LeftStick; }
        }

        public InputControl GetControl(InputTarget inputTarget) {
            return Controls[(int) inputTarget] ?? InputControl.Null;
        }

        // Warning: this is not efficient. Don't use it unless you have to, m'kay?
        public static InputTarget GetTarget(string inputControlName) { return inputControlName.ToEnum<InputTarget>(); }

        // Warning: this is not efficient. Don't use it unless you have to, m'kay?
        public InputControl GetControl(string inputControlName) { return GetControl(GetTarget(inputControlName)); }

        public InputControl AddControl(InputTarget inputTarget, string handle) {
            var inputControl = new InputControl(handle, inputTarget);
            Controls[(int) inputTarget] = inputControl;
            return inputControl;
        }

        public void UpdateWithState(InputTarget inputTarget, bool state, ulong updateTick) {
            GetControl(inputTarget).UpdateWithState(state, updateTick);
        }

        public void UpdateWithValue(InputTarget inputTarget, float value, ulong updateTick) {
            GetControl(inputTarget).UpdateWithValue(value, updateTick);
        }

        public void PreUpdate(ulong updateTick, float deltaTime) {
            for (var i = 0; i < Controls.Length; i++) {
                InputControl control = Controls[i];
                if (control == null)
                    continue;
                control.PreUpdate(updateTick);
            }
        }

        public virtual void Update(ulong updateTick, float deltaTime) {
            // Implemented by subclasses.
        }

        public void PostUpdate(ulong updateTick, float deltaTime) {
            // Apply post-processing to controls.
            for (var i = 0; i < Controls.Length; i++) {
                InputControl control = Controls[i];
                if (control == null)
                    continue;
                if (control.RawValue != null)
                    control.UpdateWithValue(control.RawValue.Value, updateTick);
                else if (control.PreValue != null)
                    control.UpdateWithValue(ProcessAnalogControlValue(control, deltaTime), updateTick);

                control.PostUpdate(updateTick);

                if (control.HasChanged)
                    LastChangeTick = updateTick;
            }

            // Update two-axis controls.
            LeftStick.Update(LeftStickX, LeftStickY, updateTick);
            RightStick.Update(RightStickX, RightStickY, updateTick);

            Vector2 dpv = DPadVector;
            DPad.Update(dpv.x, dpv.y, updateTick);
        }

        float ProcessAnalogControlValue(InputControl control, float deltaTime) {
            float analogValue = control.PreValue.Value;

            InputTarget? obverseTarget = control.Obverse;
            if (obverseTarget.HasValue) {
                InputControl obverseControl = GetControl(obverseTarget.Value);
                if (obverseControl.PreValue.HasValue) {
                    analogValue = ApplyCircularDeadZone(analogValue,
                        obverseControl.PreValue.Value,
                        control.LowerDeadZone,
                        control.UpperDeadZone);
                }
                else {
                    analogValue = ApplyDeadZone(analogValue, control.LowerDeadZone, control.UpperDeadZone);
                }
            }
            else {
                analogValue = ApplyDeadZone(analogValue, control.LowerDeadZone, control.UpperDeadZone);
            }

            return ApplySmoothing(analogValue, control.LastValue, deltaTime, control.Sensitivity);
        }

        static float ApplyDeadZone(float value, float lowerDeadZone, float upperDeadZone) {
            return Mathf.InverseLerp(lowerDeadZone, upperDeadZone, Mathf.Abs(value)) * Mathf.Sign(value);
        }

        static float ApplyCircularDeadZone(float axisValue1, float axisValue2, float lowerDeadZone, float upperDeadZone) {
            var axisVector = new Vector2(axisValue1, axisValue2);
            float magnitude = Mathf.InverseLerp(lowerDeadZone, upperDeadZone, axisVector.magnitude);
            return (axisVector.normalized * magnitude).x;
        }

        static float ApplySmoothing(float thisValue, float lastValue, float deltaTime, float sensitivity) {
            // 1.0f and above is instant (no smoothing).
            if (Mathf.Approximately(sensitivity, 1.0f))
                return thisValue;

            // Apply sensitivity (how quickly the value adapts to changes).
            float maxDelta = deltaTime * sensitivity * 100.0f;

            // Snap to zero when changing direction quickly.
            if (Mathf.Sign(lastValue) != Mathf.Sign(thisValue))
                lastValue = 0.0f;

            return Mathf.MoveTowards(lastValue, thisValue, maxDelta);
        }

        public bool LastChangedAfter(InputDevice otherDevice) { return LastChangeTick > otherDevice.LastChangeTick; }

        public virtual void Vibrate(float leftMotor, float rightMotor) { }

        public void Vibrate(float intensity) { Vibrate(intensity, intensity); }

    }

}