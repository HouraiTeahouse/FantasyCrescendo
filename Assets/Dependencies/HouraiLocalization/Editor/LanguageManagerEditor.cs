using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;

namespace HouraiTeahouse.Localization.Editor {

    using Editor = UnityEditor.Editor;

    [CustomEditor(typeof(LanguageManager))]
    public class LanguageManagerEditor : Editor {

        private string[] availableLanguages;
        private int _index;

        private Regex _splitCamelCase;

        void OnEnable() {
            _splitCamelCase = new Regex("([A-Z])");
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            if (!EditorApplication.isPlayingOrWillChangePlaymode) 
                return;
            var langManager = target as LanguageManager;

            if (availableLanguages == null) {
                availableLanguages = langManager.AvailableLanguages.Select(lang => _splitCamelCase.Replace(lang, " $1")).ToArray();
                _index = Array.LastIndexOf(availableLanguages, _splitCamelCase.Replace(langManager.CurrentLangauge.name, " $1"));
            }

            int oldIndex = _index;
            _index = EditorGUILayout.Popup("Current Language", _index, availableLanguages);
            if (_index != oldIndex) {
                langManager.LoadLanguage(availableLanguages[_index]);
            }
        }

    }

}
