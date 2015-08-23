using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    [RequireComponent(typeof(Text))]
    public class CharacterNameDisplay : MonoBehaviour, ICharacterGUIComponent {

        [SerializeField]
        private bool toUpperCase = true;

        private Text _text;

        public void SetCharacterData(CharacterMatchData data) {
            if (_text == null)
                return;
            if (data == null || data.Data == null)
                _text.text = "";
            else
                _text.text = ProcessName(data.Data.FirstName);
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