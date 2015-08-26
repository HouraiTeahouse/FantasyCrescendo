using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    [RequireComponent(typeof(Text))]
    public class PlayerNameDisplay : MonoBehaviour, IPlayerGUIComponent {

        [SerializeField]
        private bool toUpperCase = true;

        private Text _text;

        public void SetPlayerData(Player data) {
            if (_text == null)
                return;
            if (data == null || data.Character == null)
                _text.text = "";
            else
                _text.text = ProcessName(data.Character.FirstName);
        }

        string ProcessName(string raw) {
            if (toUpperCase)
                raw = raw.ToUpper();
            return raw;
        }

        void Awake() {
            _text = GetComponent<Text>();
        }

    }

}