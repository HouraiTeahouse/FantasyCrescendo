using System;
using System.Collections.Generic;
using UnityEngine;
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
    public sealed class LanguageManager : Singleton<LanguageManager> {
        [SerializeField, Tooltip("Destroy this object on scene changes?")]
        private bool _dontDestroyOnLoad = false;

        [SerializeField, Tooltip("The Resources directory to load the Language files from")]
        private string localizaitonResourceDirectory = "Lang/";

        [SerializeField, Tooltip("The PlayerPrefs key to store the Player's language in")]
        private PrefString _langPlayerPref;

        [SerializeField, Tooltip("The default language to use if the Player's current language is not supported")]
        [Resource(typeof (StringSet))]
        private string _defaultLanguage;

        [SerializeField, Tooltip("The set of keys to use")]
        private StringSet _keys;

        private Language _currentLanguage;

#if HOURAI_EVENTS
        private Mediator _eventManager;
#endif

        /// <summary>
        /// The currently used language.
        /// </summary>
        public Language CurrentLangauge {
            get { return _currentLanguage; }
        }

        /// <summary>
        /// An event that is called every time the language is changed.
        /// </summary>
        public event Action<Language> OnChangeLanguage;

        private HashSet<string> _languages;
        private HashSet<string> _keySet;

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
            get { return _keySet; }
        }

        /// <summary>
        /// Is the provided key localizable?
        /// </summary>
        /// <param name="key">the key to check</param>
        /// <returns>True if the key will return a localized string, false otherwise.</returns>
        public bool HasKey(string key) {
            return _keySet.Contains(key);
        }

        void SetLanguage(string name, StringSet set) {
            if (_currentLanguage.Name == name)
                return;
            _currentLanguage.Update(_keys, set);
            _currentLanguage.Name = set.name;
            OnChangeLanguage.SafeInvoke(_currentLanguage);
#if HOURAI_EVENTS
            _eventManager.Publish(new LanguageEvent {NewLanguage = _currentLanguage});
#endif
        }

        protected override void Awake() {
            base.Awake();

            _currentLanguage = new Language();
#if HOURAI_EVENTS
            _eventManager = GlobalMediator.Instance;
#endif

            var languages = new List<StringSet>(Resources.LoadAll<StringSet>(localizaitonResourceDirectory));
            languages.Remove(_keys);
            _languages = new HashSet<string>(languages.Select(lang => lang.name));
            _keySet = new HashSet<string>(_keys);

            string currentLang;
            if (!_langPlayerPref.HasKey()) {
                currentLang = Application.systemLanguage.ToString();
                if (!_languages.Contains(currentLang) || Application.systemLanguage == SystemLanguage.Unknown)
                    currentLang = Resources.Load<StringSet>(_defaultLanguage).name;
                _langPlayerPref.Value = currentLang;
            }
            else {
                currentLang = _langPlayerPref;
            }

            foreach (StringSet lang in languages) {
                if (lang.name == currentLang)
                    SetLanguage(lang.name, lang);
                else
                    Resources.UnloadAsset(lang);
            }

            if (_dontDestroyOnLoad)
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
                _langPlayerPref.DeleteKey();
            else
                _langPlayerPref.Value = CurrentLangauge.Name;
        }

        /// <summary>
        /// Localizes a key based on the currently loaded language.
        /// </summary>
        /// <param name="key">the localization key to use.</param>
        /// <returns>the localized string</returns>
        public string this[string key] {
            get { return CurrentLangauge[key]; }
        }

        /// <summary>
        /// Loads a new language given the Microsoft language identifier.
        /// </summary>
        /// <param name="identifier">the Microsoft identifier for a lanuguage</param>
        /// <exception cref="ArgumentNullException">throw if <paramref name="identifier"/> is null.</exception>
        /// <exception cref="InvalidOperationException">the language specified by <paramref name="identifier"/> is not currently supported.</exception>
        /// <returns>the localization language</returns>
        public Language LoadLanguage(string identifier) {
            Check.NotNull("identifier", identifier);
            if (!_languages.Contains(identifier))
                throw new InvalidOperationException("Language with identifier of {0} is not supported.".With(
                    identifier));
            var languageValues = Resources.Load<StringSet>(localizaitonResourceDirectory + identifier);
            SetLanguage(languageValues.name, languageValues);
            Resources.UnloadAsset(languageValues);
            return CurrentLangauge;
        }
    }
}
