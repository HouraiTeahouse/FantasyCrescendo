using UnityEngine;
using UnityEngine.UI;

namespace Hourai.Localization {

    [DisallowMultipleComponent]
    public class LocalizedText : MonoBehaviour {

        [SerializeField]
        private string _key;

        [SerializeField]
        private Text _text;

        private LanguageManager _languageManager;

        public Text Text {
            get { return _text; }
            set { _text = value; }
        }

        public string Key {
            get { return _key; }
            set {
                _key = value;
                if (_text && _languageManager && _languageManager.HasKey(_key))
                    _text.text = Process(_languageManager[_key]);
            }
        }

        /// <summary>
        /// Unity Callback. Called once upon object instantiation.
        /// </summary>
        protected virtual void Awake() {
            _languageManager = LanguageManager.Instance;
            if (!_text)
                _text = GetComponent<Text>();
            enabled = _languageManager && _text;
        }

        /// <summary>
        /// Unity Callback. Called on the first frame before Update is called.
        /// </summary>
        protected virtual void Start() {
            if (!_languageManager.HasKey(_key)) {
                Debug.LogWarning(string.Format("Tried to localize key {0}, but LanguageManager has no such key", _key));
            }
            _languageManager.OnChangeLanguage += OnChangeLanguage;
            _text.text = Process(_languageManager[_key]);
        }

        /// <summary>
        /// Event callback for when the system wide language is changed.
        /// </summary>
        /// <param name="language">the language set that was changed to.</param>
        void OnChangeLanguage(Language language) {
            if (!language)
                return;
            if (!language.ContainsKey(_key)) {
                Debug.LogWarning(string.Format("Tried to localize key {0}, but langauge {1} has no such key", _key, language));
                return;
            }
            _text.text = Process(_languageManager[_key]);
        }

        protected virtual string Process(string val) {
            return val;
        }

    }

}