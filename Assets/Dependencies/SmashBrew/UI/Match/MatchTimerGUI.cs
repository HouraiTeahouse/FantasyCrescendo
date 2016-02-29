using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {
    
    /// <summary>
    /// The GUI display for the Match timer.  
    /// </summary>
    [RequireComponent(typeof(Text))]
    public sealed class MatchTimerGUI : MonoBehaviour {

        /// <summary>
        /// The UI Text object to display the time on.
        /// </summary>
        [SerializeField]
        private Text _displayText;

        /// <summary>
        /// The TimeMatch reference to check for.
        /// </summary>
        [SerializeField]
        private TimeMatch _timeMatch;

        private int currentDisplayedTime;

        /// <summary>
        /// Unity callback. Called once before object's first frame.
        /// </summary>
        void Start() {
            if(!_displayText)
                _displayText = GetComponent<Text>();
            if(!_timeMatch)
                _timeMatch = FindObjectOfType<TimeMatch>();
            enabled = _displayText && _timeMatch && _timeMatch.isActiveAndEnabled;
            _displayText.enabled = enabled;
        }

        /// <summary>
        /// Unity callback. Called once per frame.
        /// </summary>
        void Update() {
            var remainingTime = Mathf.FloorToInt(_timeMatch.CurrentTime);
            if (remainingTime == currentDisplayedTime)
                return;
            currentDisplayedTime = remainingTime;
            int seconds = remainingTime % 60;
            int minutes = remainingTime / 60;
            _displayText.text = string.Format("{0}:{1}", minutes.ToString("d2"), seconds.ToString("d2"));
        }

    }

}
