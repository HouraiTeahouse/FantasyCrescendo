using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Google.GData.Spreadsheets;
using HouraiTeahouse.Editor;
using UnityEngine.Assertions;

namespace HouraiTeahouse.Localization.Editor {

    /// <summary>
    /// A Editor-Only ScriptableObject for pulling localization data from Google Spreadsheets
    /// and creating the approriate Langauge assets.
    /// </summary>
    [HelpURL("http://wiki.houraiteahouse.net/index.php/Dev:Localization#Localization_Generator")]
    public class LocalizationGenerator : ScriptableObject {

        [MenuItem("Hourai/Localization/Generate")]
        static void Create() {
            var generator = AssetUtil.LoadFirstOrCreate<LocalizationGenerator>();
            Assert.IsNotNull(generator);
            if (generator)
                generator.Generate();
        }
        
        [SerializeField, Tooltip("The public Google Spreadsheets link to pull data from")]
        private string GoogleLink;

        [SerializeField, Tooltip("Columns in the spreadsheet to ignore")]
        private string[] _ignoreColumns;

        [SerializeField, Tooltip("The folder to save all of the generated assets into.")]
        private Object _saveFolder;

        /// <summary>
        /// Reads the Google Spreadsheet and generates/updates the StringSet asset files
        /// </summary>
        public void Generate() {
            ListFeed test = GDocService.GetSpreadsheet(GoogleLink);
            var languageMap = new Dictionary<string, StringSet>();
            var keys = CreateInstance<StringSet>();
            languageMap.Add("Keys", keys);
            var ignore = new HashSet<string>(_ignoreColumns);
            foreach (ListEntry row in test.Entries) {
                keys.Add(row.Title.Text);
                foreach (ListEntry.Custom element in row.Elements) {
                    string lang = element.LocalName;
                    if (ignore.Contains(lang))
                        continue;
                    if (!languageMap.ContainsKey(lang))
                        languageMap[lang] = CreateInstance<StringSet>();
                    languageMap[lang].Add(element.Value);
                }
            }
            string folderPath = _saveFolder ? AssetDatabase.GetAssetPath(_saveFolder) : "Assets/Resources/Lang";
            foreach (var lang in languageMap) {
                var method = "Generating";
                string path = string.Format("{0}/{1}.asset", folderPath, lang.Key);
                var language = AssetDatabase.LoadAssetAtPath<StringSet>(path);
                if (language) {
                    method = "Updating";
                    language.Copy(lang.Value);
                    EditorUtility.SetDirty(language);
               }
                else
                    AssetDatabase.CreateAsset(lang.Value, path);
                Log.Info("{0} language files for: {1}", method, lang.Key);
            }
            EditorApplication.SaveAssets();
            AssetDatabase.SaveAssets();
        }

    }

}
