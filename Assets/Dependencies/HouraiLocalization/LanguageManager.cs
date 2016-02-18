using System;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System.Linq;

namespace HouraiTeahouse.Localization {

    [HelpURL("http://wiki.houraiteahouse.net/index.php/Dev:Localization#Language_Manager")]
    public sealed  class LanguageManager : MonoBehaviour {

        [SerializeField]
        private string localizaitonResourceDirectory = "Lang/";

        [SerializeField]
        private string _langPlayerPrefKey = "lang";

        [SerializeField]
        private bool _dontDestroyOnLoad = false;

        [SerializeField, Resource(typeof(Language))]
        private string _defaultLanguage;

        private Language _currentLanguage;

        /// <summary>
        /// The currently used language.
        /// </summary>
        public Language CurrentLangauge {
            get { return _currentLanguage;}
            private set {
                bool changed = _currentLanguage != value;
                _currentLanguage = value;
                if (changed && OnChangeLanguage != null)
                    OnChangeLanguage(value);
            }
        }

        /// <summary>
        /// The Singleton instance of LanguamgeManager.
        /// </summary>
        public static LanguageManager Instance { get; private set; }

        /// <summary>
        /// An event that is called every time the language is changed.
        /// </summary>
        public event Action<Language> OnChangeLanguage;

        private HashSet<string> _languages;
        private HashSet<string> _keys; 

        /// <summary>
        /// All available languages currently supported by the system.
        /// </summary>
        public IEnumerable<string> AvailableLanguages {
            get { return _languages; }
        }

        /// <summary>
        /// Is the provided key localizable?
        /// </summary>
        /// <param name="key">the key to check</param>
        /// <returns>True if the key will return a localized string, false otherwise.</returns>
        public bool HasKey(string key) {
            return _keys.Contains(key);
        }

        public static CultureInfo GetCultureInfo(SystemLanguage language) {
            switch (language) {
                case SystemLanguage.Afrikaans:
                    return new CultureInfo("af");
                case SystemLanguage.Arabic:
                    return new CultureInfo("ar");
                case SystemLanguage.Basque:
                    return new CultureInfo("eu");
                case SystemLanguage.Belarusian:
                    return new CultureInfo("be");
                case SystemLanguage.Bulgarian:
                    return new CultureInfo("bg");
                case SystemLanguage.Catalan:
                    return new CultureInfo("ca");
                case SystemLanguage.Chinese:
                    return new CultureInfo("zh-cn");
                case SystemLanguage.Czech:
                    return new CultureInfo("cs");
                case SystemLanguage.Danish:
                    return new CultureInfo("da");
                case SystemLanguage.Dutch:
                    return new CultureInfo("nl");
                case SystemLanguage.English:
                    return new CultureInfo("en");
                case SystemLanguage.Estonian:
                    return new CultureInfo("et");
                case SystemLanguage.Faroese:
                    return new CultureInfo("fo");
                case SystemLanguage.Finnish:
                    return new CultureInfo("fi");
                case SystemLanguage.French:
                    return new CultureInfo("fr");
                case SystemLanguage.German:
                    return new CultureInfo("de");
                case SystemLanguage.Greek:
                    return new CultureInfo("el");
                case SystemLanguage.Hebrew:
                    return new CultureInfo("he");
                case SystemLanguage.Icelandic:
                    return new CultureInfo("is");
                case SystemLanguage.Indonesian:
                    return new CultureInfo("id");
                case SystemLanguage.Italian:
                    return new CultureInfo("it");
                case SystemLanguage.Japanese:
                    return new CultureInfo("ja");
                case SystemLanguage.Korean:
                    return new CultureInfo("ko");
                case SystemLanguage.Latvian:
                    return new CultureInfo("lv");
                case SystemLanguage.Lithuanian:
                    return new CultureInfo("lt");
                case SystemLanguage.Norwegian:
                    return new CultureInfo("no");
                case SystemLanguage.Polish:
                    return new CultureInfo("pl");
                case SystemLanguage.Portuguese:
                    return new CultureInfo("pt");
                case SystemLanguage.Romanian:
                    return new CultureInfo("ro");
                case SystemLanguage.Russian:
                    return new CultureInfo("ru");
                case SystemLanguage.SerboCroatian:
                    return new CultureInfo("hr");
                case SystemLanguage.Slovak:
                    return new CultureInfo("sk");
                case SystemLanguage.Slovenian:
                    return new CultureInfo("sl");
                case SystemLanguage.Spanish:
                    return new CultureInfo("es");
                case SystemLanguage.Swedish:
                    return new CultureInfo("sv");
                case SystemLanguage.Thai:
                    return new CultureInfo("th");
                case SystemLanguage.Turkish:
                    return new CultureInfo("tr");
                case SystemLanguage.Ukrainian:
                    return new CultureInfo("uk");
                case SystemLanguage.Vietnamese:
                    return new CultureInfo("vi");
                case SystemLanguage.ChineseSimplified:
                    return new CultureInfo("zh-chs");
                case SystemLanguage.ChineseTraditional:
                    return new CultureInfo("zh-cht");
                case SystemLanguage.Hungarian:
                    return new CultureInfo("hu");
                default:
                    return CultureInfo.InvariantCulture;
            }
        }

        /// <summary>
        /// Unity Callback. Called once on object instantation.
        /// </summary>
        void Awake() {
            if (Instance) {
                Destroy(this);
                return;
            }
            Instance = this;

            Language[] languages = Resources.LoadAll<Language>(localizaitonResourceDirectory);
            _languages = new HashSet<string>(languages.Select(lang => lang.name));
            _keys = new HashSet<string>(languages.SelectMany(lang => lang.Keys));

            string currentLang;
            if (!Prefs.HasKey(_langPlayerPrefKey)) {
                currentLang = Application.systemLanguage.ToString();
                if (!_languages.Contains(currentLang) || Application.systemLanguage == SystemLanguage.Unknown)
                    currentLang = Resources.Load<Language>(_defaultLanguage).name;
                Prefs.SetString(_langPlayerPrefKey, currentLang);
            } else {
                currentLang = Prefs.GetString(_langPlayerPrefKey);
            }

            foreach (Language lang in languages) {
                if (lang.name == currentLang)
                    CurrentLangauge = lang;
                else
                    Resources.UnloadAsset(lang);
            }

            if(_dontDestroyOnLoad)
                DontDestroyOnLoad(this);
        }

        /// <summary>
        /// Unity Callback. Called on object destruction.
        /// </summary>
        void OnDestroy() {
            Save();
        }

        /// <summary>
        /// Unity Callback. Called when entire application exits.
        /// </summary>
        void OnApplicationQuit() {
            Save();
        }

        /// <summary>
        /// Saves the current language preferences to PlayerPrefs to keep it persistent.
        /// </summary>
        void Save() {
            Prefs.SetString(_langPlayerPrefKey, CurrentLangauge.name);
        }

        /// <summary>
        /// Localizes a key based on the currently loaded language.
        /// </summary>
        /// <param name="key">the localization key to use.</param>
        /// <returns>the localized string</returns>
        public string this[string key] {
            get {
                if(!CurrentLangauge)
                    throw new InvalidOperationException();
                return CurrentLangauge[key];
            }
        }

        /// <summary>
        /// Loads a new language given a CultureInfo instance.
        /// </summary>
        /// <param name="language">the culture information for a language.</param>
        /// <exception cref="ArgumentNullException">throw if <paramref name="language"/> is null.</exception>
        /// <exception cref="InvalidOperationException">thrown if the language specified by <paramref name="language"/> is not currently supported.</exception>>
        /// <returns>the localization language</returns>
        public Language LoadLanguage(CultureInfo language) {
            if(language == null)
                throw  new ArgumentNullException("language");
            while (!language.IsNeutralCulture)
                language = language.Parent;
            if(!_languages.Contains(language.ToString()))
                throw new InvalidOperationException(string.Format("Language {0} is not supported at this time.", language.EnglishName));
            return LoadLanguage(language.ToString());
        }

        /// <summary>
        /// Loads a new language given the Microsoft language identifier.
        /// </summary>
        /// <param name="identifier">the Microsoft identifier for a lanuguage</param>
        /// <exception cref="ArgumentNullException">throw if <paramref name="identifier"/> is null.</exception>
        /// <exception cref="ArgumentException">thrown if <paramref name="identifier"/> does not correspond to any known language</exception>
        /// <exception cref="InvalidOperationException">thrown if the language specified by <paramref name="identifier"/> is not currently supported.</exception>
        /// <returns>the localization language</returns>
        public Language LoadLanguage(string identifier) {
            if (identifier == null)
                throw new ArgumentNullException("identifier");
            if (!_languages.Contains(identifier))
                throw new InvalidOperationException(string.Format("Language with identifier of {0} is not supported.", identifier));
            CurrentLangauge = Resources.Load<Language>(localizaitonResourceDirectory + identifier);
            return CurrentLangauge;
        }

    }

}
