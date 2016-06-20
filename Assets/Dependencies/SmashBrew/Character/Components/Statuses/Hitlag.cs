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

namespace HouraiTeahouse.SmashBrew {
    /// <summary> A Status effect that keeps a character frozen for a short amount of time after being hit. </summary>
    [RequiredCharacterComponent]
    [DisallowMultipleComponent]
    public class Hitlag : Status {
        float _cachedTimeScale;

        /// <summary>
        ///     <see cref="Status.GetDeltaTime" />
        /// </summary>
        protected override float GetDeltaTime() {
            return Time.unscaledDeltaTime;
        }

        /// <summary>
        ///     <see cref="Status.OnStatusStart" />
        /// </summary>
        protected override void OnStatusStart() {
            base.OnStatusStart();
            _cachedTimeScale = LocalTimeScale;
            LocalTimeScale = 0f;
        }

        /// <summary>
        ///     <see cref="Status.OnStatusEnd" />
        /// </summary>
        protected override void OnStatusEnd() {
            base.OnStatusEnd();
            LocalTimeScale = _cachedTimeScale;
        }
    }
}