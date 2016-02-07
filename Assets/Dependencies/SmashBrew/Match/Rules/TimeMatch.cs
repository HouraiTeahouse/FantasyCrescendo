using UnityEngine;
using Hourai.Events;

namespace Hourai.SmashBrew {

    /// <summary>
    /// A MatchRule that adds a time limit.
    /// The match ends the instant the timer hits zero.
    /// 
    /// Note this rule does not determine a winner, only ends the Match.
    /// </summary>
    public class TimeMatch : MatchRule {

        [SerializeField]
        private float _time = 180f;

        /// <summary>
        /// The amount of time remaining in the Match, in seconds.
        /// </summary>
        public float CurrentTime { get; private set; }

        /// <summary>
        /// Gets the winner of the Match. Null if the rule does not declare one.
        /// </summary>
        /// <remarks>TimeMatch doesn't determine winners, so this will always be null.</remarks>
        /// <returns>the winner of the match. Always null.</returns>
        public override Player GetWinner() {
            return null;
        }

        private Mediator _eventManager;

        /// <summary>
        /// Unity Callback. Called on object instantiation.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            _eventManager = GlobalMediator.Instance;
            _eventManager.Subscribe<MatchStartEvent>(OnMatchStart);
        }
        
        /// <summary>
        /// Event callback. Called when the Match starts and ends.
        /// </summary>
        /// <param name="startEventArgs">the event parameters</param>
        void OnMatchStart(MatchStartEvent startEventArgs) {
            CurrentTime = _time;
        }

        /// <summary>
        /// Unity Callback. Called once every frame.
        /// </summary>
        void Update() {
            CurrentTime -= Time.unscaledDeltaTime;
            if(CurrentTime <= 0)
                Match.FinishMatch();
        }

    }

}
