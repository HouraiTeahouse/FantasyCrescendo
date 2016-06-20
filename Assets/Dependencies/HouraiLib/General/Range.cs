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

namespace HouraiTeahouse {
    /// <summary> A simple struct representing a range of real numbers. </summary>
    public struct Range {
        float _min;
        float _max;

        /// <summary> The lower bound of the Range </summary>
        public float Min {
            get { return _min; }
            set {
                if (value > Max) {
                    _min = _max;
                    _max = value;
                }
                else {
                    _min = value;
                }
            }
        }

        /// <summary> The upper bound of the Range </summary>
        public float Max {
            get { return _max; }
            set {
                if (value < Min) {
                    _max = _min;
                    _min = value;
                }
                else {
                    _min = value;
                }
            }
        }

        /// <summary> Returns the width of the Range. Equal to the difference between Max and Min. </summary>
        public float Width {
            get { return _max - _min; }
        }

        /// <summary> Returns the center of the Range </summary>
        public float Center {
            get { return Lerp(0.5f); }
        }

        /// <summary> Gets the full range of real numbers, from negative infinity to positive infinity </summary>
        public static Range FullRange {
            get {
                return new Range(float.NegativeInfinity, float.PositiveInfinity);
            }
        }

        /// <summary> Creates an instance of Range. </summary>
        /// <param name="value"> </param>
        public Range(float value) : this(value, value) {
        }

        /// <summary> Creates a instance of Range. </summary>
        /// <param name="min"> the </param>
        /// <param name="max"> </param>
        public Range(float min, float max) {
            _min = Mathf.Min(min, max);
            _max = Mathf.Max(min, max);
        }

        /// <summary> Clamps a value to the Range </summary>
        /// <param name="x"> the value to clamp </param>
        /// <returns> the clamped value </returns>
        public float Clamp(float x) {
            return Mathf.Clamp(x, _min, _max);
        }

        /// <summary> Selects a random floating point number contianed within the Range </summary>
        /// <returns> a random number sampled from the range, with uniform probabilty </returns>
        public float Random() {
            return UnityEngine.Random.Range(_min, _max);
        }

        /// <summary> Linearly interpolates between the two extremes of the range. If val = 1, returns Max. If val = 0, returns
        /// Min. If betweeen, the returned value is linearly proportional to the width of the Range. </summary>
        /// <param name="val"> </param>
        /// <returns> </returns>
        public float Lerp(float val) {
            return _min + val * Width;
        }

        public static implicit operator Range(float f) { return new Range(f); }

        public static implicit operator Range(int f) { return new Range(f); }

        public static Range operator +(Range r1, Range r2) {
            return new Range(r1.Min + r2.Min, r1.Max + r2.Max);
        }

        public static Range operator -(Range r1, Range r2) {
            return new Range(r1.Min - r2.Min, r1.Max - r2.Max);
        }

        public static Range operator *(float f, Range r) {
            return new Range(f * r.Min, f * r.Max);
        }

        public static Range operator *(Range r, float f) {
            return new Range(f * r.Min, f * r.Max);
        }

        public static Range operator /(Range r, float f) {
            return new Range(r.Min / f, r.Max / f);
        }
    }
}