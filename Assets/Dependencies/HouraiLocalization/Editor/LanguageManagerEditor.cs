using System;
using System.Globalization;
using System.Linq;
using UnityEditor;

namespace Hourai.Localization.Editor {

    using Editor = UnityEditor.Editor;

    [CustomEditor(typeof(LanguageManager))]
    public class LanguageManagerEditor : Editor {

        private string[] availableLanguages;
        private int _index;

        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            if (!EditorApplication.isPlayingOrWillChangePlaymode) 
                return;
            var langManager = target as LanguageManager;

            if (availableLanguages == null) {
                availableLanguages = langManager.AvailableLanguages.Select(lang => lang.ToString()).ToArray();
                _index = Array.LastIndexOf(availableLanguages, CultureInfo.GetCultureInfo(langManager.CurrentLangauge.name).ToString());
            }

            int oldIndex = _index;
            _index = EditorGUILayout.Popup("Current Language", _index, availableLanguages);
            if (_index != oldIndex) {
                langManager.LoadLanguage(availableLanguages[_index]);
            }
        }

    }

}