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

    /// <summary> An abstract class  to define a Match Rule. These instances are searched for before the start of a Match to
    /// define the rules of a match. They run as normal MonoBehaviours, but are regularly polled for </summary>
    [RequireComponent(typeof(Match))]
    public abstract class MatchRule : MonoBehaviour {

        /// <summary> The PlayerPrefs key to check for whether the rule is used or not. Stored as an integer. If 0, the rule is
        /// disabled. If any other number, it is enabled. If the key does not exist. The rule remains in whatever state it was left
        /// in the editor. </summary>
        [SerializeField]
        string _playerPrefCheck;

        /// <summary> A refernce to the central Match object. </summary>
        protected static Match Match { get; private set; }

        /// <summary> Unity Callback. Called on object instantiation. </summary>
        protected virtual void Awake() {
            if (Match == null)
                Match = FindObjectOfType<Match>();
#if !UNITY_EDITOR
            if (Prefs.HasKey(_playerPrefCheck))
                enabled = Prefs.GetBool(_playerPrefCheck);
#endif
        }

        /// <summary> Gets the winner of the Match, according to the Match Rule. </summary>
        /// <returns> the Player that won. Null if there is a tie, or no winner is declared. </returns>
        public abstract Player GetWinner();

    }
    
}
