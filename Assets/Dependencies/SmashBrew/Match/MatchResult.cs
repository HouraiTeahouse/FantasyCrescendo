using UnityEngine;
using System.Collections;

namespace Hourai.SmashBrew {

    /// <summary>
    /// Enum containing the possible end results of a Match
    /// </summary>
    public enum MatchResult {

        /// <summary>
        /// No Contest. The match was ended prematurely. No winner.
        /// </summary>
        NoContest,

        /// <summary>
        /// Two or more players met the winning condition. No single winner.
        /// </summary>
        Tie,

        /// <summary>
        /// Only one player met the winning conditions. Has a single winner.
        /// </summary>
        HasWinner
    }

}
