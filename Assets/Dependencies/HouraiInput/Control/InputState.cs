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

#pragma warning disable 0660, 0661

namespace HouraiTeahouse.HouraiInput {
    public struct InputState {
        public bool State;
        public float Value;


        public void Reset() {
            Value = 0.0f;
            State = false;
        }


        public void Set(float value) {
            Value = value;
            State = !Mathf.Approximately(value, 0.0f);
        }


        public void Set(float value, float threshold) {
            Value = value;
            State = Mathf.Abs(value) > threshold;
        }


        public void Set(bool state) {
            State = state;
            Value = state ? 1.0f : 0.0f;
        }


        public static implicit operator bool(InputState state) {
            return state.State;
        }


        public static implicit operator float(InputState state) {
            return state.Value;
        }


        public static bool operator ==(InputState a, InputState b) {
            return Mathf.Approximately(a.Value, b.Value);
        }


        public static bool operator !=(InputState a, InputState b) {
            return !Mathf.Approximately(a.Value, b.Value);
        }
    }
}