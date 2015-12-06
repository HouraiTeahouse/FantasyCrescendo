using UnityEngine;
using Hourai.Events;

namespace Hourai.SmashBrew {

    public class TimeMatch : MatchRule {

        [SerializeField]
        private float _time = 180f;

        public float CurrentTime { get; private set; }

        protected override bool IsFinished {
            get { return isActiveAndEnabled && CurrentTime <= 0; }
        }

        public override Player Winner {
            get { return null; }
        }

        private Mediator _eventManager;

        protected override void Awake() {
            base.Awake();
            _eventManager = GlobalEventManager.Instance;
            _eventManager.Subscribe<MatchEvent>(OnMatchStart);
        }

        void OnMatchStart(MatchEvent eventArgs) {
            if (!eventArgs.Start)
                return;
            CurrentTime = _time;
        }

        protected override void Update() {
            base.Update();
            CurrentTime -= Time.deltaTime;
        }

    }

}