using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> The GUI display for the Match timer. </summary>
    [RequireComponent(typeof(Text))]
    public sealed class MatchTimerGUI : MonoBehaviour {

        /// <summary> The UI Text object to display the time on. </summary>
        [SerializeField]
        Text _displayText;

        /// <summary> The TimeMatch reference to check for. </summary>
        [SerializeField]
        TimeMatch _timeMatch;

        int currentDisplayedTime;

        /// <summary> Unity callback. Called once before object's first frame. </summary>
        void Start() {
            if (!_displayText)
                _displayText = GetComponent<Text>();
            if (!_timeMatch)
                _timeMatch = FindObjectOfType<TimeMatch>();
            enabled = _displayText && _timeMatch && _timeMatch.isActiveAndEnabled;
            _displayText.enabled = enabled;
        }

        /// <summary> Unity callback. Called once per frame. </summary>
        void Update() {
            const string format = "d2";
            int remainingTime = Mathf.FloorToInt(_timeMatch.CurrentTime);
            if (remainingTime == currentDisplayedTime)
                return;
            currentDisplayedTime = remainingTime;
            int seconds = remainingTime % 60;
            int minutes = remainingTime / 60;
            _displayText.text = "{0}:{1}".With(minutes.ToString(format), seconds.ToString(format));
        }

    }

}