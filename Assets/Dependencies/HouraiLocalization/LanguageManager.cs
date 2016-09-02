using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.Localization {

    public class LanguageEvent {

        public Language NewLanguage;

    }

    /// <summary> Singleton MonoBehaviour that manages all of localization system. </summary>
    public sealed class LanguageManager : Singleton<LanguageManager> {

        Language _currentLanguage;

        [SerializeField]
        [Tooltip("The default language to use if the Player's current language is not supported")]
        [Resource(typeof(StringSet))]
        string _defaultLanguage;

        [SerializeField]
        [Tooltip("Destroy this object on scene changes?")]
        bool _dontDestroyOnLoad = false;

#if HOURAI_EVENTS
        Mediator _eventManager;
#endif

        [SerializeField]
        [Tooltip("The set of keys to use")]
        StringSet _keys;

        HashSet<string> _keySet;

        [SerializeField]
        [Tooltip("The PlayerPrefs key to store the Player's language in")]
        PrefString _langPlayerPref;

        HashSet<string> _languages;

        [SerializeField]
        [Tooltip("The Resources directory to load the Language files from")]
        string localizaitonResourceDirectory = "Lang/";

        /// <summary> The currently used language. </summary>
        public Language CurrentLangauge {
            get { return _currentLanguage; }
        }

        /// <summary> All available languages currently supported by the system. </summary>
        public IEnumerable<string> AvailableLanguages {
            get {
                if (_languages != null)
                    return _languages;
                return new string[0];
            }
        }

        /// <summary> Gets an enumeration of all of the localizable keys. </summary>
        public IEnumerable<string> Keys {
            get { return _keySet; }
        }

        /// <summary> Localizes a key based on the currently loaded language. </summary>
        /// <param name="key"> the localization key to use. </param>
        /// <returns> the localized string </returns>
        public string this[string key] {
            get { return CurrentLangauge[key]; }
        }

        /// <summary> An event that is called every time the language is changed. </summary>
        public event Action<Language> OnChangeLanguage;

        /// <summary> Is the provided key localizable? </summary>
        /// <param name="key"> the key to check </param>
        /// <returns> True if the key will return a localized string, false otherwise. </returns>
        public bool HasKey(string key) { return _keySet.Contains(key); }

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
            _eventManager = Mediator.Global;
#endif

            var languages = new List<StringSet>(Resources.LoadAll<StringSet>(localizaitonResourceDirectory));
            languages.Remove(_keys);
            _languages = new HashSet<string>(languages.Select(lang => lang.name));
            _keySet = new HashSet<string>(_keys);

            SystemLanguage systemLang = Application.systemLanguage;
            string currentLang = _langPlayerPref.HasKey() ? _langPlayerPref : systemLang.ToString();
            if (!_languages.Contains(currentLang) || systemLang == SystemLanguage.Unknown) {
                string oldLang = currentLang;
                currentLang = Resources.Load<StringSet>(_defaultLanguage).name;
                Log.Info("No language data for \"{0}\" found. Loading default language: {1}", oldLang, currentLang);
            }
            _langPlayerPref.Value = currentLang;

            foreach (StringSet lang in languages) {
                if (lang.name == currentLang)
                    SetLanguage(lang.name, lang);
                else
                    Resources.UnloadAsset(lang);
            }

            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(this);
        }

        /// <summary> Unity Callback. Called on object destruction. </summary>
        void OnDestroy() { Save(); }

        /// <summary> Unity Callback. Called when entire application exits. </summary>
        void OnApplicationQuit() { Save(); }

        /// <summary> Saves the current language preferences to PlayerPrefs to keep it persistent. </summary>
        void Save() {
            if (CurrentLangauge == null)
                _langPlayerPref.DeleteKey();
            else
                _langPlayerPref.Value = CurrentLangauge.Name;
        }

        /// <summary> Loads a new language given the Microsoft language identifier. </summary>
        /// <param name="identifier"> the Microsoft identifier for a lanuguage </param>
        /// <returns> the localization language </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="identifier" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> the language specified by <paramref name="identifier" /> is not currently
        /// supported. </exception>
        public Language LoadLanguage(string identifier) {
            Check.NotNull(identifier);
            if (!_languages.Contains(identifier))
                throw new InvalidOperationException("Language with identifier of {0} is not supported.".With(identifier));
            var languageValues = Resources.Load<StringSet>(localizaitonResourceDirectory + identifier);
            SetLanguage(languageValues.name, languageValues);
            Resources.UnloadAsset(languageValues);
            return CurrentLangauge;
        }

    }

}