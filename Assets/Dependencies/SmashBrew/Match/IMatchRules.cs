using UnityEngine;

namespace Hourai.SmashBrew {
    
    [RequireComponent(typeof(Match))]
    public abstract class MatchRule : MonoBehaviour{

        protected abstract bool IsFinished { get; }
        public abstract Player Winner { get; }

        protected Match Match { get; private set; }

        [SerializeField]
        private string _playerPrefCheck;

        protected virtual void Awake() {
            Match = GetComponent<Match>();
#if !UNITY_EDITOR
            if (!Prefs.HasKey(_playerPrefCheck))
                enabled = false;
            else
                enabled = Prefs.GetBool(_playerPrefCheck) != 0;
#endif
        }

        protected virtual void Update() {
            if(IsFinished)
                Match.FinishMatch();
        }

    }

}
