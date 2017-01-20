using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.Localization {

    /// <summary> An abstract MonoBehaviour class that localizes the strings displayed on UI Text objects. </summary>
    public abstract class AbstractLocalizedText : MonoBehaviour {

        [SerializeField]
        Text _text;

        string _nativeText;

        /// <summary> The UI Text object to display the localized string onto </summary>
        public Text Text {
            get { return _text; }
            set { _text = value; }
        }

        /// <summary> The localization key used to lookup the localized string. </summary>
        protected string NativeText {
            get { return _nativeText; }
            set {
                if (value == null || !_text)
                    return;
                _nativeText = value;
                LanguageManager languageManager = LanguageManager.Instance;
                _text.text = Process(languageManager[_nativeText]);
            }
        }

        /// <summary> Unity Callback. Called once upon object instantiation. </summary>
        protected virtual void Awake() {
            if (!_text)
                _text = this.SafeGetComponent<Text>();
            enabled = _text;
        }

        /// <summary> Unity Callback. Called on the first frame before Update is called. </summary>
        protected virtual void Start() {
            LanguageManager languageManager = LanguageManager.Instance;
            if (languageManager == null)
                return;
            languageManager.OnChangeLanguage += OnChangeLanguage;
            if (_nativeText == null)
                return;
            _text.text = Process(languageManager[_nativeText]);
        }

        protected virtual void OnDestroy() {
            LanguageManager languageManager = LanguageManager.Instance;
            if (languageManager != null)
                languageManager.OnChangeLanguage -= OnChangeLanguage;
        }

        /// <summary> Events callback for when the system wide language is changed. </summary>
        /// <param name="language"> the language set that was changed to. </param>
        void OnChangeLanguage(Language language) {
            if (language == null || _nativeText == null)
                return;
            _text.text = Process(language[_nativeText]);
        }

        /// <summary> Post-Processing on the retrieved localized string. </summary>
        /// <param name="val"> the pre-processed localized string </param>
        /// <returns> the post-processed localized string </returns>
        protected virtual string Process(string val) { return val; }

    }

    /// <summary> An AbstractLocalizedText where the localization key is defined via serializaiton </summary>
    [HelpURL("http://wiki.houraiteahouse.net/index.php/Dev:Localization#Localized_Text")]
    public sealed class LocalizedText : AbstractLocalizedText {

        /// <summary> The format for the localization string to be displayed in. </summary>
        /// <see cref="string.Format" />
        [SerializeField]
        string _format;

        /// <summary> The serialized localization key </summary>
        [SerializeField]
        string _key;

        /// <summary> Gets or sets the localization key of the LocalizedText </summary>
        public string Key {
            get { return NativeText; }
            set { NativeText = value; }
        }

        /// <summary> Unity callback. Called once before the object's first frame. </summary>
        protected override void Awake() {
            base.Awake();
            if (Text && string.IsNullOrEmpty(_key))
                _key = Text.text;
            NativeText = _key;
        }

        /// <summary>
        ///     <see cref="AbstractLocalizedText" />
        /// </summary>
        protected override string Process(string val) { return _format.IsNullOrEmpty() ? val : _format.With(val); }

    }

}
