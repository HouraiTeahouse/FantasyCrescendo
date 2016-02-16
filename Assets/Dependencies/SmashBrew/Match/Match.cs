using HouraiTeahouse.Events;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    
    /// <summary>
    /// The Match Singleton.
    /// 
    /// Manages the current state of the match and all of the defined Match rules.
    /// </summary>
    public class Match : MonoBehaviour {

        private Mediator _eventManager;

        /// <summary>
        /// Unity Callback. Called on object instantiation.
        /// </summary>
        void Awake() {
            _eventManager = GlobalMediator.Instance;
        }

        /// <summary>
        /// Ends the match.
        /// </summary>
        /// <param name="noContest">is the match ending prematurely? If set to true, no
        ///     winner will be declared.</param>
        public void FinishMatch(bool noContest = false) {
            var rules = FindObjectsOfType<MatchRule>();

            var result = MatchResult.HasWinner;
            Player winner = null;
            foreach (MatchRule rule in rules) {
                if (rule == null)
                    continue;
                rule.enabled = false;
                Player ruleWinner = rule.GetWinner();
                if(ruleWinner == null || noContest)
                    continue;
                if (winner == null) {
                    // No other winner has been declared yet
                    winner = ruleWinner;
                } else {
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

        /// <summary>
        /// Gets whether there is currently a Match underway.
        /// </summary>
        public bool InMatch { get; private set; }

        /// <summary>
        /// Unity Callback. Called before the object's first frame.
        /// </summary>
        void Start() {
            // Don't restart a match if it is still in progress
            if (InMatch)
                return;
            _eventManager.Publish(new MatchStartEvent());
        }

    }

}
