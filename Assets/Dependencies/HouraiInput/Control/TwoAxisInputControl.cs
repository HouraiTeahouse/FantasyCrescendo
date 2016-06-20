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
    public class TwoAxisInputControl {
        public static float StateThreshold = 0.0f;
        bool lastState;

        bool thisState;


        internal TwoAxisInputControl() {
            Left = new OneAxisInputControl();
            Right = new OneAxisInputControl();
            Up = new OneAxisInputControl();
            Down = new OneAxisInputControl();
        }

        public float X { get; protected set; }
        public float Y { get; protected set; }

        public OneAxisInputControl Left { get; protected set; }
        public OneAxisInputControl Right { get; protected set; }
        public OneAxisInputControl Up { get; protected set; }
        public OneAxisInputControl Down { get; protected set; }

        public ulong UpdateTick { get; protected set; }


        public bool State {
            get { return thisState; }
        }


        public bool HasChanged {
            get { return thisState != lastState; }
        }


        public Vector2 Vector {
            get { return new Vector2(X, Y); }
        }


        internal void Update(float x, float y, ulong updateTick) {
            lastState = thisState;

            X = x;
            Y = y;

            Left.UpdateWithValue(Mathf.Clamp01(-X), updateTick, StateThreshold);
            Right.UpdateWithValue(Mathf.Clamp01(X), updateTick, StateThreshold);

            if (HInput.InvertYAxis) {
                Up.UpdateWithValue(Mathf.Clamp01(-Y), updateTick, StateThreshold);
                Down.UpdateWithValue(Mathf.Clamp01(Y),
                    updateTick,
                    StateThreshold);
            }
            else {
                Up.UpdateWithValue(Mathf.Clamp01(Y), updateTick, StateThreshold);
                Down.UpdateWithValue(Mathf.Clamp01(-Y),
                    updateTick,
                    StateThreshold);
            }

            thisState = Up.State || Down.State || Left.State || Right.State;

            if (thisState != lastState) {
                UpdateTick = updateTick;
            }
        }


        public static implicit operator bool(TwoAxisInputControl control) {
            return control.thisState;
        }


        public static implicit operator Vector2(TwoAxisInputControl control) {
            return control.Vector;
        }


        public static implicit operator Vector3(TwoAxisInputControl control) {
            return new Vector3(control.X, control.Y);
        }
    }
}