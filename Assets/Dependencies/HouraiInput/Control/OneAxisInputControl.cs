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

namespace HouraiTeahouse.HouraiInput {

    public class OneAxisInputControl {

        InputState lastState;
        InputState thisState;

        public ulong UpdateTick { get; private set; }

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

        public void UpdateWithValue(float value,
                                    ulong updateTick,
                                    float stateThreshold) {
            if (UpdateTick > updateTick)
                throw new InvalidOperationException("A control cannot be updated with an earlier tick.");
            lastState = thisState;
            thisState.Set(value, stateThreshold);
            if (thisState != lastState)
                UpdateTick = updateTick;
        }

        public static implicit operator bool(OneAxisInputControl control) {
            return control.State;
        }

        public static implicit operator float(OneAxisInputControl control) {
            return control.Value;
        }

    }
}
