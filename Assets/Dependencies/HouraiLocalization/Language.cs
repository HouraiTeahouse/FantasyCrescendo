using System.Collections.Generic;

namespace HouraiTeahouse.Localization {

    /// <summary> A class of ScriptableObjects that simply stores a set of String-String key-value pairs corresponding to the
    /// localization keys and the respective localized strings for that one particular language. Specially created to be saved
    /// as an asset file that can be loaded dynamically via Resources. Cannot be created through the editor, must be generated
    /// with LocalizationGenerator. </summary>
    public sealed class Language {

        readonly Dictionary<string, string> _map;

        /// <summary> Initializes an empty instance of Language </summary>
        public Language() { _map = new Dictionary<string, string>(); }

        /// <summary> Creates an instance of Language from two sets of keys and values </summary>
        /// <param name="keys"> the localization keys for the language </param>
        /// <param name="values"> the values of the Language </param>
        public Language(IEnumerable<string> keys, IEnumerable<string> values) : this() { Update(keys, values); }

        public string Name { get; set; }

        /// <summary> Gets an enumeration of all of the localization keys supported by the Language </summary>
        public IEnumerable<string> Keys {
            get { return _map.Keys; }
        }

        /// <summary> Gets a localized string for a specific localization key. </summary>
        /// <exception cref="KeyNotFoundException"> the Language does not support the localization key </exception>
        /// <param name="key"> the localization key to retrieve </param>
        /// <returns> the localized string </returns>
        public string this[string key] {
            get {
                if (!_map.ContainsKey(key))
                    throw new KeyNotFoundException(key);
                return _map[key];
            }
        }

        /// <summary> Updates the current Language from two sets of keys and values </summary>
        /// <param name="keys"> the localization keys for the language </param>
        /// <param name="values"> the values of the Language </param>
        public void Update(IEnumerable<string> keys, IEnumerable<string> values) {
            _map.Clear();
            if (keys == null || values == null)
                return;
            IEnumerator<string> keyEnum = keys.GetEnumerator();
            IEnumerator<string> valuesEnum = values.GetEnumerator();
            while (keyEnum.MoveNext() && valuesEnum.MoveNext())
                if (keyEnum.Current != null)
                    _map[keyEnum.Current] = valuesEnum.Current;
        }

        /// <summary> Checks if the Langauge contains a specific localization key. </summary>
        /// <param name="key"> the localizaiton key to check for </param>
        /// <returns> True if the Langauge can localize the key, false otherwise. </returns>
        public bool ContainsKey(string key) { return _map.ContainsKey(key); }

    }

}