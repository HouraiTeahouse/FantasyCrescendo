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

namespace HouraiTeahouse {
    /// <summary> Static class for editing the global time properties of the game. Allows for pausing of the game and altering
    /// the global time scale. Inherits from MonoBehaviour. A custom Editor allows editing the pause/timescale state of the
    /// game from the Editor. </summary>
    public class TimeManager : Singleton<TimeManager> {
        static float _timeScale = 1f;
        static bool _paused;

        /// <summary> Gets or sets whether the game is paused or not. Changing this value will fire the OnPause event If the value
        /// is the same, nothing will change. If the game is paused, Time.timeScale will be set to 0. When unpaused, Time.timeScale
        /// will be set to the value of TimeScale </summary>
        public static bool Paused {
            get { return _paused; }
            set {
                if (_paused == value)
                    return;
                _paused = value;
                Time.timeScale = _paused ? 0f : TimeScale;
                if (OnPause != null)
                    OnPause();
            }
        }

        /// <summary> Gets or sets the global timescale of the game. If the game is not paused, Time.timeScale will also be set to
        /// the same value </summary>
        public static float TimeScale {
            get { return _timeScale; }
            set {
                if (Mathf.Approximately(_timeScale, value))
                    return;
                _timeScale = value;
                if (!Paused)
                    Time.timeScale = value;
                OnTimeScaleChange.SafeInvoke();
            }
        }

        /// <summary> Events. Called every time the game is paused or unpaused. </summary>
        public static event Action OnPause;

        /// <summary> Events. Called every time the global timescale is changed. </summary>
        public static event Action OnTimeScaleChange;

        /// <summary> Unity callback. Called on object instantiation. </summary>
        protected override void Awake() {
            base.Awake();
            _timeScale = Time.timeScale;
        }
    }
}
