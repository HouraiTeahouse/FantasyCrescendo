using Google.GData.Spreadsheets;
using HouraiTeahouse.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace HouraiTeahouse.Localization.Editor {

    /// <summary> A Editor-Only ScriptableObject for pulling localization data from Google Spreadsheets and creating the
    /// approriate Langauge assets. </summary>
    public class LocalizationGenerator : ScriptableObject {

        [SerializeField]
        [Tooltip("The public Google Spreadsheets link to pull data from")]
        string GoogleLink;

        [SerializeField]
        [Tooltip("The default language to use")]
        string _defaultLanguage;

        [SerializeField]
        [Tooltip("Columns in the spreadsheet to ignore")]
        string[] _ignoreColumns;

        [SerializeField]
        [Tooltip("The folder to save all of the generated assets into.")]
        Object _saveFolder;

        [MenuItem("Hourai Teahouse/Localization/Generate")]
        static void Create() {
            var generator = Assets.LoadOrCreate<LocalizationGenerator>();
            Assert.IsNotNull(generator);
            if (generator)
                generator.Generate();
        }

        /// <summary> Reads the Google Spreadsheet and generates/updates the StringSet asset files </summary>
        public void Generate() {
            ListFeed test = GDocService.GetSpreadsheet(GoogleLink);
            var languageMap = new Dictionary<string, Dictionary<string, string>>();
            var ignore = new HashSet<string>(_ignoreColumns);
            foreach (ListEntry row in test.Entries) {
                var keyEntry =
                    row.Elements.OfType<ListEntry.Custom>()
                        .FirstOrDefault(e => e.LocalName.ToLower() == _defaultLanguage.ToLower());
                if (keyEntry == null)
                    continue;
                var key = keyEntry.Value;
                foreach (ListEntry.Custom element in row.Elements) {
                    string lang = element.LocalName;
                    if (ignore.Contains(lang))
                        continue;
                    if (!languageMap.ContainsKey(lang))
                        languageMap[lang] = new Dictionary<string, string>();
                    languageMap[lang].Add(key, element.Value);
                }
            }
            var baseFolder = Path.Combine(Application.streamingAssetsPath, "lang");
            if (!Directory.Exists(baseFolder))
                Directory.CreateDirectory(baseFolder);
            foreach (var lang in languageMap) {
                File.WriteAllText(Path.Combine(baseFolder, lang.Key + LanguageManager.FileExtension),
                    JsonConvert.SerializeObject(lang.Value, Formatting.Indented));
                Log.Info("Generating language files for: {0}", lang.Key);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

}
