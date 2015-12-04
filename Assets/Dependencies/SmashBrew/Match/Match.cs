using Hourai.Events;
using UnityEngine;

namespace Hourai.SmashBrew {

    public class Match : MonoBehaviour {

        private Mediator eventManager; 

        void Awake() {
            eventManager = GlobalEventManager.Instance;
        }

        public class MatchEvent : IEvent {

            public bool start;

        }

        public void FinishMatch(Character winner) {

            eventManager.Publish(new MatchEvent { start = false });
        }

        public bool InMatch { get; private set; }

        void Start() {

            // Don't restart a match if it is still in progress
            if (InMatch)
                return;

            eventManager.Publish(new MatchEvent { start = true });
        }

    }

}