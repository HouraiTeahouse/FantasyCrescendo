using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Matches {

    /// <summary> The Match Singleton. Manages the current state of the match and all of the defined Match rules. </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Smash Brew/Matches/Match")]
    public class Match : NetworkBehaviour {

        Mediator _eventManager;
        [SyncVar]
        bool _isRunning;

        /// <summary> Unity Callback. Called on object instantiation. </summary>
        void Awake() { _eventManager = Mediator.Global; }

        public override void OnStartServer() {
            _eventManager.Publish(new MatchStartEvent());
            _isRunning = true;
        }

        /// <summary> Ends the match. </summary>
        /// <param name="noContest"> is the match ending prematurely? If set to true, no winner will be declared. </param>
        public void FinishMatch(bool noContest) {
            if (!_isRunning)
                return;
            MatchRule[] rules = FindObjectsOfType<MatchRule>();

            var result = MatchResult.HasWinner;
            Player winner = null;
            foreach (MatchRule rule in rules) {
                if (rule == null)
                    continue;
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
            _isRunning = false;
        }

    }

}
