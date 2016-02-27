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

        private string[] availableLanguages;
        private string[] display;
        private int _index;

        private Regex _splitCamelCase;

        protected override void OnEnable() {
            base.OnEnable();
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
                _index = Array.LastIndexOf(availableLanguages, langManager.CurrentLangauge.name);
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
