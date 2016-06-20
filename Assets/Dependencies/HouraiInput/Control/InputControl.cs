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
using UnityEngine;

namespace HouraiTeahouse.HouraiInput {
    public class InputControl {
        public static readonly InputControl Null =
            new InputControl("NullInputControl");

        InputState lastState;
        internal float? PreValue;

        // This is for internal use only and is not always set.
        internal float? RawValue;
        InputState tempState;

        InputState thisState;

        ulong zeroTick;


        InputControl(string handle) {
            Handle = handle;
            Sensitivity = 1.0f;
            LowerDeadZone = 0.0f;
            UpperDeadZone = 1.0f;
        }


        public InputControl(string handle, InputTarget target) {
            Handle = handle;
            Target = target;

            IsButton = (target >= InputTarget.Action1
                && target <= InputTarget.Action4)
                || (target >= InputTarget.Button0
                    && target <= InputTarget.Button19);
        }

        public string Handle { get; protected set; }
        public InputTarget Target { get; protected set; }

        public ulong UpdateTick { get; protected set; }

        public float Sensitivity { get; set; }
        public float LowerDeadZone { get; set; }
        public float UpperDeadZone { get; set; }

        /// <summary> Is this control a button? </summary>
        public bool IsButton { get; protected set; }


        internal bool IsOnZeroTick {
            get { return UpdateTick == zeroTick; }
        }


        public bool State {
            get { return thisState.State; }
        }

        public bool LastState {
            get { return lastState.State; }
        }

        public float Value {
            get { return thisState.Value; }
        }

        public float LastValue {
            get { return lastState.Value; }
        }

        public bool HasChanged {
            get { return thisState != lastState; }
        }

        public bool IsPressed {
            get { return thisState.State; }
        }

        public bool WasPressed {
            get { return thisState && !lastState; }
        }

        public bool WasReleased {
            get { return !thisState && lastState; }
        }

        public bool IsNull {
            get { return this == Null; }
        }

        public bool IsNotNull {
            get { return this != Null; }
        }

        public InputTarget? Obverse {
            get {
                switch (Target) {
                    case InputTarget.LeftStickX:
                        return InputTarget.LeftStickY;
                    case InputTarget.LeftStickY:
                        return InputTarget.LeftStickX;
                    case InputTarget.RightStickX:
                        return InputTarget.RightStickY;
                    case InputTarget.RightStickY:
                        return InputTarget.RightStickX;
                    default:
                        return null;
                }
            }
        }


        public void UpdateWithState(bool state, ulong updateTick) {
            if (IsNull) {
                throw new InvalidOperationException(
                    "A null control cannot be updated.");
            }

            if (UpdateTick > updateTick) {
                throw new InvalidOperationException(
                    "A control cannot be updated with an earlier tick.");
            }

            tempState.Set(state || tempState.State);
        }


        public void UpdateWithValue(float value, ulong updateTick) {
            if (IsNull) {
                throw new InvalidOperationException(
                    "A null control cannot be updated.");
            }

            if (UpdateTick > updateTick) {
                throw new InvalidOperationException(
                    "A control cannot be updated with an earlier tick.");
            }

            if (Mathf.Abs(value) > Mathf.Abs(tempState.Value)) {
                tempState.Set(value);
            }
        }


        internal void PreUpdate(ulong updateTick) {
            RawValue = null;
            PreValue = null;

            lastState = thisState;
            tempState.Reset();
        }


        internal void PostUpdate(ulong updateTick) {
            thisState = tempState;
            if (thisState != lastState) {
                UpdateTick = updateTick;
            }
        }


        internal void SetZeroTick() { zeroTick = UpdateTick; }

        public override string ToString() {
            return "[InputControl: Handle={0}, Value={1}]".With(Handle, Value);
        }

        public static implicit operator bool(InputControl control) {
            return control.State;
        }

        public static implicit operator float(InputControl control) {
            return control.Value;
        }
    }
}