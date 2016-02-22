using System;
using System.Linq;
using System.Text.RegularExpressions;
using HouraiTeahouse.Editor;
using UnityEditor;

namespace HouraiTeahouse.Localization.Editor {

    [CustomEditor(typeof(LanguageManager))]
    public class LanguageManagerEditor : ScriptlessEditor {

        private string[] availableLanguages;
        private string[] display;
        private int _index;

        private Regex _splitCamelCase;

        protected override void OnEnable() {
            base.OnEnable();
            _splitCamelCase = new Regex(".([A-Z])");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (!EditorApplication.isPlayingOrWillChangePlaymode) 
                return;
            var langManager = target as LanguageManager;

            if (availableLanguages == null) {
                availableLanguages = langManager.AvailableLanguages.ToArray();
                display = availableLanguages.Select(lang => _splitCamelCase.Replace(lang, " $1")).ToArray();
                _index = Array.LastIndexOf(availableLanguages, langManager.CurrentLangauge.name);
            }

            int oldIndex = _index;
            _index = EditorGUILayout.Popup("Current Language", _index, display);
            if (_index != oldIndex) {
                langManager.LoadLanguage(availableLanguages[_index]);
            }
        }

    }

}
