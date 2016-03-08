using System;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System.Linq;
#if HOURAI_EVENTS
using HouraiTeahouse.Events;
#endif

namespace HouraiTeahouse.Localization {

#if HOURAI_EVENTS
    public class LanguageEvent {

        public Language NewLanguage;

    }
#endif
    /// <summary>
    /// Singleton MonoBehaviour that manages all of localization system.
    /// </summary>
    [HelpURL("http://wiki.houraiteahouse.net/index.php/Dev:Localization#Language_Manager")]
    public sealed class LanguageManager : MonoBehaviour {

        [SerializeField, Tooltip("The Resources directory to load the Language files from")]
        private string localizaitonResourceDirectory = "Lang/";

        [SerializeField, Tooltip("The PlayerPrefs key to store the Player's language in") ]
        private string _langPlayerPrefKey = "lang";

        [SerializeField, Tooltip("Destroy this object on scene changes?")]
        private bool _dontDestroyOnLoad = false;

        [SerializeField, Tooltip("The default language to use if the Player's current language is not supported")]
        [Resource(typeof(Language))]
        private string _defaultLanguage;

        private Language _currentLanguage;

#if HOURAI_EVENTS
        private Mediator _eventManager;
#endif

        /// <summary>
        /// The currently used language.
        /// </summary>
        public Language CurrentLangauge {
            get { return _currentLanguage;}
            private set {
                bool changed = _currentLanguage != value;
                if(_currentLanguage != null)
                    Resources.UnloadAsset(_currentLanguage);
                _currentLanguage = value;
                if (changed && OnChangeLanguage != null)
                    OnChangeLanguage(value);
#if HOURAI_EVENTS
                _eventManager.Publish(new LanguageEvent { NewLanguage = value });
#endif
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
            get {
                if (_languages != null)
                    return _languages;
                return new string[0];
            }
        }

        /// <summary>
        /// Gets an enumeration of all of the localizable keys.  
        /// </summary>
        public IEnumerable<string> Keys {
            get {
                foreach (var key in _keys)
                    yield return key;
            }
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
        /// Converts a SystemLanugage value into a CultureInfo.
        /// </summary>
        /// <param name="language">the SystemLanugage value to map</param>
        /// <returns>the corresponding CultureInfo</returns>
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
            Instance = this;

#if HOURAI_EVENTS
            _eventManager = GlobalMediator.Instance;
#endif

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
            if (CurrentLangauge == null)
                PlayerPrefs.DeleteKey(_langPlayerPrefKey);
            else
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
        /// Loads a new language given the Microsoft language identifier.
        /// </summary>
        /// <param name="identifier">the Microsoft identifier for a lanuguage</param>
        /// <exception cref="ArgumentNullException">throw if <paramref name="identifier"/> is null.</exception>
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
