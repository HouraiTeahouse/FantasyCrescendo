using System;
using UnityEngine;
using System.Collections.Generic;

namespace Hourai.Localization {

    public class Language : ScriptableObject, ISerializationCallbackReceiver {

        [System.Serializable]
        private struct StrStrTuple {

            public string Key;
            public string Value;

        }

        [SerializeField]
        private StrStrTuple[] data;

        private Dictionary<string, string> _map;

        public string this[string key] {
            get {
                if(!_map.ContainsKey(key))
                    throw new KeyNotFoundException(key);
                return _map[key];
            }
        }

        public static Language FromDictionary(Dictionary<string, string> src) {
            var lang = CreateInstance<Language>();
            if(src != null)
                lang.ReadFromDictionary(src);
            return lang;
        }

        public void ReadFromDictionary(Dictionary<string, string> src) {
            if(src == null)
                throw new ArgumentNullException("src");
            data = new StrStrTuple[src.Count];
            var i = 0;
            foreach (KeyValuePair<string, string> kvp in src) {
                data[i].Key = kvp.Key;
                data[i].Value = kvp.Value;
                i++;
            }
        }

        public Dictionary<string, string> ToDictionary() {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var sst in data) {
                dict[sst.Key] = sst.Value;
            }
            return dict;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            if(_map != null)
                ReadFromDictionary(_map);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            _map = ToDictionary();
        }

    }

}