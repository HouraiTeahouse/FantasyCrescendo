using System;
using System.Linq;
using System.Text.RegularExpressions;
using HouraiTeahouse.Editor;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.Localization.Editor {

    /// <summary>
    /// A custom Editor for LanguageManager.
    /// </summary>
    [CustomEditor(typeof(LanguageManager))]
    internal class LanguageManagerEditor : ScriptlessEditor {

        string[] availableLanguages;
        string[] display;
        int _index;

        Regex _splitCamelCase;

        void OnEnable() {
            _splitCamelCase = new Regex(".([A-Z])");
        }

        /// <summary>
        /// <see cref="UnityEditor.Editor.OnInspectorGUI"/>
        /// </summary>
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var langManager = target as LanguageManager;

            if (availableLanguages == null) {
                availableLanguages = langManager.AvailableLanguages.ToArray();
                display = availableLanguages.Select(lang => _splitCamelCase.Replace(lang, " $1")).ToArray();
                Language language = langManager.CurrentLangauge;
                if(language != null)
                    _index = Array.LastIndexOf(availableLanguages, langManager.CurrentLangauge.Name);
            }

            GUI.enabled = EditorApplication.isPlayingOrWillChangePlaymode;
            int oldIndex = _index;
            _index = EditorGUILayout.Popup("Current Language", _index, display);
            if (_index != oldIndex)
                langManager.LoadLanguage(availableLanguages[_index]);
            GUI.enabled = true;
        }

    }

}
