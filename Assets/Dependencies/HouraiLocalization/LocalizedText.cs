using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.Localization {

    public abstract class AbstractLocalizedText : MonoBehaviour {

        [SerializeField]
        private Text _text;

        private LanguageManager _languageManager;
        private string _localizationKey;

        public Text Text {
            get { return _text; }
            set { _text = value; }
        }

        protected string LocalizationKey {
            get { return _localizationKey; }
            set {
                bool changed = _localizationKey == value;
                _localizationKey = value;
                if (!changed || _localizationKey == null)
                    return;
                if (_languageManager.HasKey(_localizationKey))
                    _text.text = Process(_languageManager[_localizationKey]);
                else
                    Debug.LogWarning(string.Format("Tried to localize key {0}, but LanguageManager has no such key", _localizationKey));
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
            _languageManager.OnChangeLanguage += OnChangeLanguage;
            if (_localizationKey == null)
                return;
            if (_languageManager.HasKey(_localizationKey))
                _text.text = Process(_languageManager[_localizationKey]);
            else
                Debug.LogWarning(string.Format("Tried to localize key {0}, but LanguageManager has no such key", _localizationKey));
        }

        /// <summary>
        /// Event callback for when the system wide language is changed.
        /// </summary>
        /// <param name="language">the language set that was changed to.</param>
        void OnChangeLanguage(Language language) {
            if (!language)
                return;
            if (_localizationKey == null)
                return;
            if (language.ContainsKey(_localizationKey))
                _text.text = Process(language[_localizationKey]);
            else
                Debug.LogWarning(string.Format("Tried to localize key {0}, but langauge {1} has no such key", _localizationKey, language));
        }

        protected virtual string Process(string val) {
            return val;
        }

    }

    [HelpURL("http://wiki.houraiteahouse.net/index.php/Dev:Localization#Localized_Text")]
    public sealed class LocalizedText : AbstractLocalizedText {

        [SerializeField]
        private string _key;

        protected override void Start() {
            base.Start();
            LocalizationKey = _key;
        }

        public string Key {
            get { return LocalizationKey; }
            set { LocalizationKey = value; }
        }

    }

}
