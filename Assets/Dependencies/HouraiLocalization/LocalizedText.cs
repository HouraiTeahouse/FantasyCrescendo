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

using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.Localization {
    /// <summary> An abstract MonoBehaviour class that localizes the strings displayed on UI Text objects. </summary>
    public abstract class AbstractLocalizedText : MonoBehaviour {
        string _localizationKey;

        [SerializeField]
        Text _text;

        /// <summary> The UI Text object to display the localized string onto </summary>
        public Text Text {
            get { return _text; }
            set { _text = value; }
        }

        /// <summary> The localization key used to lookup the localized string. </summary>
        protected string LocalizationKey {
            get { return _localizationKey; }
            set {
                if (_localizationKey == value || value == null || !_text)
                    return;
                _localizationKey = value;
                LanguageManager languageManager = LanguageManager.Instance;
                if (languageManager.HasKey(_localizationKey))
                    _text.text = Process(languageManager[_localizationKey]);
                else
                    Log.Warning(
                        "Tried to localize key {0}, but LanguageManager has no such key",
                        LocalizationKey);
            }
        }

        /// <summary> Unity Callback. Called once upon object instantiation. </summary>
        protected virtual void Awake() {
            if (!_text)
                _text = GetComponent<Text>();
            enabled = _text;
        }

        /// <summary> Unity Callback. Called on the first frame before Update is called. </summary>
        protected virtual void Start() {
            LanguageManager languageManager = LanguageManager.Instance;
            if (languageManager == null)
                return;
            languageManager.OnChangeLanguage += OnChangeLanguage;
            if (_localizationKey == null)
                return;
            if (languageManager.HasKey(_localizationKey))
                _text.text = Process(languageManager[_localizationKey]);
            else
                Log.Warning(
                    "Tried to localize key {0}, but LanguageManager has no such key",
                    _localizationKey);
        }

        /// <summary> Events callback for when the system wide language is changed. </summary>
        /// <param name="language"> the language set that was changed to. </param>
        void OnChangeLanguage(Language language) {
            if (language == null || _localizationKey == null)
                return;
            if (language.ContainsKey(_localizationKey))
                _text.text = Process(language[_localizationKey]);
            else
                Log.Warning(
                    "Tried to localize key {0}, but langauge {1} has no such key",
                    _localizationKey,
                    language);
        }

        /// <summary> Post-Processing on the retrieved localized string. </summary>
        /// <param name="val"> the pre-processed localized string </param>
        /// <returns> the post-processed localized string </returns>
        protected virtual string Process(string val) {
            return val;
        }
    }

    /// <summary> An AbstractLocalizedText where the localization key is defined via serializaiton </summary>
    [HelpURL(
        "http://wiki.houraiteahouse.net/index.php/Dev:Localization#Localized_Text"
        )]
    public sealed class LocalizedText : AbstractLocalizedText {
        /// <summary> The format for the localization string to be displayed in. </summary>
        /// <see cref="string.Format" />
        [SerializeField]
        string _format;

        /// <summary> The serialized localization key </summary>
        [SerializeField]
        string _key;

        /// <summary> Gets or sets the localization key of the LocalizedText </summary>
        public string Key {
            get { return LocalizationKey; }
            set { LocalizationKey = value; }
        }

        /// <summary> Unity callback. Called once before the object's first frame. </summary>
        protected override void Awake() {
            base.Awake();
            LocalizationKey = _key;
        }

        /// <summary>
        ///     <see cref="AbstractLocalizedText" />
        /// </summary>
        protected override string Process(string val) {
            return _format.IsNullOrEmpty() ? val : _format.With(val);
        }
    }
}
