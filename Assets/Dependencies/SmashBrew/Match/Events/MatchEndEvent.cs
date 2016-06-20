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

namespace HouraiTeahouse.SmashBrew {
    /// <summary> Events for declaring the end of the Match </summary>
    public class MatchEndEvent {
        /// <summary> The result of the Match. Whether there is a winner, a tie, or no contest. </summary>
        public readonly MatchResult MatchResult;

        /// <summary> The declared winner of the match. If a tie, or it was a No-Contest, this will be null. </summary>
        public readonly Player Winner;

        /// <summary> Creates an instance of MatchEndEvent. </summary>
        /// <param name="matchResult"> the Match's end result </param>
        /// <param name="winner"> the winner for the Match </param>
        internal MatchEndEvent(MatchResult matchResult, Player winner) {
            MatchResult = matchResult;
            Winner = winner;
        }
    }
}