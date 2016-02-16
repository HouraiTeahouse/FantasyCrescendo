using UnityEngine;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    [RequireComponent(typeof(Text))]
    public class MatchTimerGUI : MonoBehaviour {

        private Text _displayText;
        private TimeMatch _timeMatch;
        private int currentDisplayedTime;

        void Start() {
            _displayText = GetComponent<Text>();
            _timeMatch = FindObjectOfType<TimeMatch>();
            enabled = _timeMatch && _timeMatch.isActiveAndEnabled;
            _displayText.enabled = enabled;
        }

        void Update() {
            var remainingTime = (int) _timeMatch.CurrentTime;
            if (remainingTime == currentDisplayedTime)
                return;
            currentDisplayedTime = remainingTime;
            int seconds = remainingTime % 60;
            int minutes = remainingTime / 60;
            _displayText.text = string.Format("{0}:{1}", minutes.ToString("d2"), seconds.ToString("d2"));
        }

    }

}