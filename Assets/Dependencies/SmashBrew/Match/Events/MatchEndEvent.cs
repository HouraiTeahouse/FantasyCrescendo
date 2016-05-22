namespace HouraiTeahouse.SmashBrew {
    /// <summary>
    /// Events for declaring the end of the Match
    /// </summary>
    public class MatchEndEvent {
        /// <summary>
        /// The result of the Match. Whether there is a winner, a tie, or no contest.
        /// </summary>
        public readonly MatchResult MatchResult;

        /// <summary>
        /// The declared winner of the match. If a tie, or it was a No-Contest, this will be null.
        /// </summary>
        public readonly Player Winner;

        /// <summary>
        /// Creates an instance of MatchEndEvent.
        /// </summary>
        /// <param name="matchResult">the Match's end result</param>
        /// <param name="winner">the winner for the Match</param>
        internal MatchEndEvent(MatchResult matchResult, Player winner) {
            MatchResult = matchResult;
            Winner = winner;
        }
    }
}
