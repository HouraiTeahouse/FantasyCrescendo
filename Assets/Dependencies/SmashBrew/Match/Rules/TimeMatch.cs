using UnityEngine;
using System.Collections;
using Hourai.Events;

namespace Hourai.SmashBrew {

    public class TimeMatch : MatchRule {

        [SerializeField]
        private float _time = 180f;

        public float CurrentTime { get; private set; }

        protected override bool IsFinished {
            get { return isActiveAndEnabled && CurrentTime <= 0; }
        }

        public override Character Winner {
            get { return null; }
        }

        private Mediator _eventManager;

        protected override void Awake() {
            base.Awake();
            _eventManager = GlobalEventManager.Instance;
            _eventManager.Subscribe<Match.MatchEvent>(OnMatchStart);
        }

        void OnMatchStart(Match.MatchEvent eventArgs) {
            if (!eventArgs.start)
                return;
            CurrentTime = _time;
        }

        protected override void Update() {
            base.Update();
            CurrentTime -= Time.deltaTime;
        }

    }

}