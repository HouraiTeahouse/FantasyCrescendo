// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.Localization {
    /// <summary> A class of ScriptableObjects that simply stores a set of String-String key-value pairs corresponding to the
    /// localization keys and the respective localized strings for that one particular language. Specially created to be saved
    /// as an asset file that can be loaded dynamically via Resources. Cannot be created through the editor, must be generated
    /// with LocalizationGenerator. </summary>
    [HelpURL(
        "http://wiki.houraiteahouse.net/index.php/Dev:Localization#Language_Asset"
        )]
    public sealed class Language {
        readonly Dictionary<string, string> _map;

        /// <summary> Initializes an empty instance of Language </summary>
        public Language() {
            _map = new Dictionary<string, string>();
        }

        /// <summary> Creates an instance of Language from two sets of keys and values </summary>
        /// <param name="keys"> the localization keys for the language </param>
        /// <param name="values"> the values of the Language </param>
        public Language(IEnumerable<string> keys, IEnumerable<string> values)
            : this() {
            Update(keys, values);
        }

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
        public bool ContainsKey(string key) {
            return _map.ContainsKey(key);
        }
    }
}
