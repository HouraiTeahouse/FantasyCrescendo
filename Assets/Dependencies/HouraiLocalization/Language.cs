using System;
using UnityEngine;
using System.Collections.Generic;

namespace Hourai.Localization {

    [HelpURL("http://wiki.houraiteahouse.net/index.php/Dev:Localization#Language_Asset")]
    public class Language : ScriptableObject, ISerializationCallbackReceiver {

        [Serializable]
        private struct StrStrTuple {

            public string Key;

            [Multiline]
            public string Value;

        }

        [SerializeField]
        private StrStrTuple[] data;

        /// <summary>
        /// Gets an enumeration of all of the localization keys supported by the Language
        /// </summary>
        public IEnumerable<string> Keys {
            get
            {
                if (_map == null)
                    _map = ToDictionary();
                return _map.Keys;
            }
        }

        /// <summary>
        /// Checks if the Langauge contains a specific localization key.
        /// </summary>
        /// <param name="key">the localizaiton key to check for</param>
        /// <returns>True if the Langauge can localize the key, false otherwise.</returns>
        public bool ContainsKey(string key) {
            return _map != null && _map.ContainsKey(key);
        }

        private Dictionary<string, string> _map;

        /// <summary>
        /// Gets a localized string for a specific localization key.
        /// </summary>
        /// <exception cref="KeyNotFoundException">thrown if the Language does not 
        ///     support the localization key</exception>
        /// <param name="key">the localization key to retrieve</param>
        /// <returns>the localized string</returns>
        public string this[string key] {
            get
            {
                if (_map == null)
                    _map = ToDictionary();
                if (!_map.ContainsKey(key))
                    throw new KeyNotFoundException(key);
                return _map[key];
            }
        }

        /// <summary>
        /// Creates a Language instance from a dictionary.
        /// Returns an empty Language if <paramref name="src"/> is null.
        /// </summary>
        /// <param name="src">the source dictionary</param>
        /// <returns>the new Language instance</returns>
        public static Language FromDictionary(Dictionary<string, string> src) {
            var lang = CreateInstance<Language>();
            if(src != null)
                lang.ReadFromDictionary(src);
            return lang;
        }

        /// <summary>
        /// Redefines the Language's values based on a dictionary input. 
        /// </summary>
        /// <exception cref="ArgumentNullException">thrown if <paramref name="src"/> is null</exception>
        /// <param name="src">the source Dictionary</param>
        public void ReadFromDictionary(Dictionary<string, string> src) {
            if(src == null)
                throw new ArgumentNullException("src");
            data = new StrStrTuple[src.Count];
            _map = new Dictionary<string, string>(src);
            var i = 0;
            foreach (KeyValuePair<string, string> kvp in src) {
                data[i].Key = kvp.Key;
                data[i].Value = kvp.Value;
                i++;
            }
        }

        /// <summary>
        /// Creates a dictionary from the Language.
        /// </summary>
        /// <returns>a dictionary containing the same keys/values as the Language</returns>
        public Dictionary<string, string> ToDictionary() {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var sst in data) {
                dict[sst.Key] = sst.Value;
            }
            return dict;
        }

        /// <summary>
        /// Callback. Called before the Language file is serialized to file.
        /// </summary>
        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            // Copy the dictionary key-value pairs into the StrStrTuples
            // before serialization. As Unity does not support generic serialziaiton.
            if(_map != null)
                ReadFromDictionary(_map);
        }

        /// <summary>
        /// Callback. Called after the Language file is deserialized from file.
        /// </summary>
        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            //Turn off this, deserialize when necessary
            //_map = ToDictionary();
        }

    }

}