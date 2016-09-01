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
    /// <summary> The Match Singleton. Manages the current state of the match and all of the defined Match rules. </summary>
    public class Match : MonoBehaviour {
        Mediator _eventManager;

        /// <summary> Gets whether there is currently a Match underway. </summary>
        public bool InMatch { get; private set; }

        /// <summary> Unity Callback. Called on object instantiation. </summary>
        void Awake() { _eventManager = GlobalMediator.Instance; }

        /// <summary> Ends the match. </summary>
        /// <param name="noContest"> is the match ending prematurely? If set to true, no winner will be declared. </param>
        public void FinishMatch(bool noContest = false) {
            MatchRule[] rules = FindObjectsOfType<MatchRule>();

            var result = MatchResult.HasWinner;
            Player winner = null;
            foreach (MatchRule rule in rules) {
                if (rule == null)
                    continue;
                rule.enabled = false;
                Player ruleWinner = rule.GetWinner();
                if (ruleWinner == null || noContest)
                    continue;
                if (winner == null) {
                    // No other winner has been declared yet
                    winner = ruleWinner;
                }
                else {
                    // Another winner has been declared, set as a tie
                    result = MatchResult.Tie;
                    winner = null;
                }
            }
            if (noContest) {
                result = MatchResult.NoContest;
                winner = null;
            }
            _eventManager.Publish(new MatchEndEvent(result, winner));
        }

        /// <summary> Unity Callback. Called before the object's first frame. </summary>
        void Start() {
            // Don't restart a match if it is still in progress
            if (InMatch)
                return;
            _eventManager.Publish(new MatchStartEvent());
            InMatch = true;
        }
    }
}
