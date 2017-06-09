using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace HouraiTeahouse.Localization {

    /// <summary> An abstract MonoBehaviour class that localizes the strings displayed on UI Text objects. </summary>
    public abstract class AbstractLocalizedText : MonoBehaviour {

        [SerializeField]
        Text _text;

        [SerializeField]
        TMP_Text _textMesh;

        string _nativeText;

        /// <summary> The UI Text object to display the localized string onto </summary>
        public Text Text {
            get { return _text; }
            set { 
                _text = value; 
                SetText();
            }
        }

        public TMP_Text TextMesh {
            get { return _textMesh; }
            set { 
                _textMesh = value; 
                SetText();
            }
        }

        /// <summary> The localization key used to lookup the localized string. </summary>
        protected string NativeText {
            get { return _nativeText; }
            set {
                _nativeText = value;
                if (value == null || !HasComponent())
                    return;
                SetText();
            }
        }

        protected bool HasComponent() {
            return _text || _textMesh;
        }

        /// <summary> Unity Callback. Called once upon object instantiation. </summary>
        protected virtual void Awake() {
            if (!HasComponent())
                ResetComponents();
            enabled = HasComponent();
        }

        protected void ResetComponents() {
            if (!HasComponent())
                _text = GetComponent<Text>();
            if (!HasComponent())
                _textMesh = GetComponent<TextMeshProUGUI>();
        }

        protected void SetText(string text = null) {
            if (string.IsNullOrEmpty(text)) {
                LanguageManager languageManager = LanguageManager.Instance;
                if (languageManager)
                    text = Process(languageManager[NativeText]);
                else
                    text = string.Empty;
            }
            text = Process(text);
            if (Text)
                Text.text = text;
            if (TextMesh)
                TextMesh.text = text;
        }

        protected string GetText() {
            string text = string.Empty;
            if (Text)
                text = Text.text;
            if (TextMesh)
                text = TextMesh.text;
            return text;
        }

        protected virtual void Reset() {
            ResetComponents();
        }

        /// <summary> Unity Callback. Called on the first frame before Update is called. </summary>
        protected virtual void Start() {
            LanguageManager languageManager = LanguageManager.Instance;
            if (languageManager == null)
                return;
            languageManager.OnChangeLanguage += OnChangeLanguage;
            if (string.IsNullOrEmpty(_nativeText))
                return;
            SetText();
        }

        protected virtual void OnDestroy() {
            LanguageManager languageManager = LanguageManager.Instance;
            if (languageManager != null)
                languageManager.OnChangeLanguage -= OnChangeLanguage;
        }

        /// <summary> Events callback for when the system wide language is changed. </summary>
        /// <param name="language"> the language set that was changed to. </param>
        void OnChangeLanguage(Language language) {
            if (language == null || NativeText == null)
                return;
            SetText(language[NativeText]);
        }

        /// <summary> Post-Processing on the retrieved localized string. </summary>
        /// <param name="val"> the pre-processed localized string </param>
        /// <returns> the post-processed localized string </returns>
        protected virtual string Process(string val) { return val; }

    }
}