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
                if (!_languages.Contains(currentLang))
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
