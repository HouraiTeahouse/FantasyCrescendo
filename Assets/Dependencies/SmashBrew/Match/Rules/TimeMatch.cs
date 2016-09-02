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
    /// <summary> A MatchRule that adds a time limit. The match ends the instant the timer hits zero. Note this rule does not
    /// determine a winner, only ends the Match. </summary>
    public sealed class TimeMatch : MatchRule {
        Mediator _eventManager;

        [SerializeField]
        float _time = 180f;

        /// <summary> The amount of time remaining in the Match, in seconds. </summary>
        public float CurrentTime { get; private set; }

        /// <summary> Gets the winner of the Match. Null if the rule does not declare one. </summary>
        /// <remarks> TimeMatch doesn't determine winners, so this will always be null. </remarks>
        /// <returns> the winner of the match. Always null. </returns>
        public override Player GetWinner() {
            return null;
        }

        /// <summary> Unity Callback. Called on object instantiation. </summary>
        protected override void Awake() {
            base.Awake();
            _eventManager = Mediator.Global;
            _eventManager.Subscribe<MatchStartEvent>(OnMatchStart);
        }

        /// <summary> Events callback. Called when the Match starts and ends. </summary>
        /// <param name="startEventArgs"> the event parameters </param>
        void OnMatchStart(MatchStartEvent startEventArgs) {
            CurrentTime = _time;
        }

        /// <summary> Unity Callback. Called once every frame. </summary>
        void Update() {
            CurrentTime -= Time.unscaledDeltaTime;
            if (CurrentTime <= 0)
                Match.FinishMatch();
        }
    }
}
