using HouraiTeahouse.SmashBrew.Matches;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> The GUI display for the Match timer. </summary>
    public sealed class MatchTimerGUI : MonoBehaviour {

        /// <summary> The UI Text object to display the time on. </summary>
        [SerializeField]
        Text _displayText;

        [SerializeField]
        TextMeshProUGUI _textMesh;

        /// <summary> The TimeMatch reference to check for. </summary>
        [SerializeField]
        TimeMatch _timeMatch;

        int currentDisplayedTime;

        void SetText(string text) {
            if (_displayText != null)
                _displayText.text = text;
            if (_textMesh != null)
                _textMesh.text = text;
        }

        /// <summary> Unity callback. Called once before object's first frame. </summary>
        void Start() {
            if (_displayText == null)
                _displayText = GetComponent<Text>();
            if (_textMesh == null)
                _textMesh = GetComponent<TextMeshProUGUI>();
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
            SetText("{0}:{1}".With(minutes.ToString(format), seconds.ToString(format)));
        }

    }

}
