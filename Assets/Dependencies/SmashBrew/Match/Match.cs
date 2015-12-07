using System.Linq;
using Hourai.Events;
using UnityEngine;

namespace Hourai.SmashBrew {

    public class MatchEvent : IEvent {

        public bool Start;

    }

    public class Match : MonoBehaviour {

        private Mediator _eventManager;
        private MatchEvent _event;

        void Awake() {
            _eventManager = GlobalEventManager.Instance;
            _event = new MatchEvent();
        }

        public void FinishMatch() {
            MatchRule[] rules = FindObjectsOfType<MatchRule>();
            
            //TODO: Store winner data somewhere
            //Player winner = (from rule in rules
            //                 where rule.isActiveAndEnabled
            //                 select rule.Winner).FirstOrDefault(player => player != null);
            foreach (MatchRule rule in rules.Where(rule => rule != null))
                rule.enabled = false;
            _event.Start = false;
            _eventManager.Publish(_event);
        }

        public bool InMatch { get; private set; }

        void Start() {
            // Don't restart a match if it is still in progress
            if (InMatch)
                return;
            _event.Start = true;
            _eventManager.Publish(_event);
        }

    }

}