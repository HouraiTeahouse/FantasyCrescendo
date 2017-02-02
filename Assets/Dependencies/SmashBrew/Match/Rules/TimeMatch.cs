using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Matches {

    /// <summary> A MatchRule that adds a time limit. The match ends the instant the timer hits zero. Note this rule does not
    /// determine a winner, only ends the Match. </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Smash Brew/Matches/Time Match")]
    public sealed class TimeMatch : MatchRule {

        Mediator _eventManager;

        [SerializeField]
        float _time = 180f;

        [SyncVar, SerializeField, ReadOnly]
        float _currentTime;

        /// <summary> The amount of time remaining in the Match, in seconds. </summary>
        public float CurrentTime {
            get { return _currentTime; }
            private set {
                if(hasAuthority)
                    _currentTime = value;
            }
        }

        /// <summary> Gets the winner of the Match. Null if the rule does not declare one. </summary>
        /// <remarks> TimeMatch doesn't determine winners, so this will always be null. </remarks>
        /// <returns> the winner of the match. Always null. </returns>
        public override Player GetWinner() { return null; }

        /// <summary> Unity Callback. Called on object instantiation. </summary>
        protected override void Start() {
            base.Start();
            _eventManager = Mediator.Global;
            _eventManager.Subscribe<MatchStartEvent>(OnMatchStart);
        }

        /// <summary> Events callback. Called when the Match starts and ends. </summary>
        /// <param name="startEventArgs"> the event parameters </param>
        void OnMatchStart(MatchStartEvent startEventArgs) { CurrentTime = _time; }

        /// <summary> Unity Callback. Called once every frame. </summary>
        void Update() {
            if (!IsActive)
                return;
            CurrentTime -= Time.unscaledDeltaTime;
            if (CurrentTime <= 0)
                Match.CmdFinishMatch(false);
        }

    }

}
