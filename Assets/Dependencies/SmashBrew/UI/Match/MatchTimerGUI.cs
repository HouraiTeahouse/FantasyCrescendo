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
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> The GUI display for the Match timer. </summary>
    [RequireComponent(typeof(Text))]
    public sealed class MatchTimerGUI : MonoBehaviour {

        /// <summary> The UI Text object to display the time on. </summary>
        [SerializeField]
        Text _displayText;

        /// <summary> The TimeMatch reference to check for. </summary>
        [SerializeField]
        TimeMatch _timeMatch;

        int currentDisplayedTime;

        /// <summary> Unity callback. Called once before object's first frame. </summary>
        void Start() {
            if (!_displayText)
                _displayText = GetComponent<Text>();
            if (!_timeMatch)
                _timeMatch = FindObjectOfType<TimeMatch>();
            enabled = _displayText && _timeMatch
                && _timeMatch.isActiveAndEnabled;
            _displayText.enabled = enabled;
        }

        /// <summary> Unity callback. Called once per frame. </summary>
        void Update() {
            const string format = "d2";
            int remainingTime = Mathf.FloorToInt(_timeMatch.CurrentTime);
            if (remainingTime == currentDisplayedTime)
                return;
            currentDisplayedTime = remainingTime;
            int seconds = remainingTime % 60;
            int minutes = remainingTime / 60;
            _displayText.text = "{0}:{1}".With(minutes.ToString(format),
                seconds.ToString(format));
        }
    }
}
