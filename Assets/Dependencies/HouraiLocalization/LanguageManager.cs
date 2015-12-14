using System;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System.Linq;

namespace Hourai.Localization {

    public sealed  class LanguageManager : MonoBehaviour {

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

        [SerializeField]
        private bool _dontDestroyOnLoad = false;

        private Dictionary<CultureInfo, Language> _languages;
        private HashSet<string> _keys; 

        /// <summary>
        /// All available languages currently supported by the system.
        /// </summary>
        public IEnumerable<CultureInfo> AvailableLanguages {
            get { return _languages.Keys; }
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
            _languages = new Dictionary<CultureInfo, Language>();
            string currentLang = CultureInfo.CurrentCulture.Name.ToLower();
            Language[] languages = Resources.LoadAll<Language>(string.Empty);
            _keys = new HashSet<string>(languages.SelectMany(lang => lang.Keys)); 
            foreach (Language lang in Resources.LoadAll<Language>(string.Empty)) {
                _languages[CultureInfo.GetCultureInfo(lang.name)] = lang;
                if (lang.name == currentLang) 
                    CurrentLangauge = lang;
                else
                    Resources.UnloadAsset(lang);
            }
            if(_dontDestroyOnLoad)
                DontDestroyOnLoad(this);
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
        /// Loads a new language given the Microsoft language identifier.
        /// </summary>
        /// <param name="identifier">the Microsoft identifier for a lanuguage</param>
        /// <exception cref="ArgumentNullException">throw if <paramref name="identifier"/> is null.</exception>
        /// <exception cref="ArgumentException">thrown if <paramref name="identifier"/> does not correspond to any known language</exception>
        /// <exception cref="InvalidOperationException">thrown if the language specified by <paramref name="identifier"/> is not currently supported.</exception>
        /// <returns>the localization language</returns>
        public Language LoadLanguage(string identifier) {
            if(identifier == null)
                throw new ArgumentNullException("identifier");
            CultureInfo culture = CultureInfo.GetCultureInfo(identifier);
            if(culture == null)
                throw new ArgumentException(string.Format("No matching language was found with the identifier \"{0}\"", identifier));
            return LoadLanguage(culture);
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
            if(!_languages.ContainsKey(language))
                throw new InvalidOperationException(string.Format("Language {0} is not supported at this time.", language.EnglishName));
            CurrentLangauge = _languages[language];
            return CurrentLangauge;
        }

    }

}