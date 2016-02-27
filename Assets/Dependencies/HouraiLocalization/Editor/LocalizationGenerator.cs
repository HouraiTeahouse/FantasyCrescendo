using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Google.GData.Spreadsheets;
using HouraiTeahouse.Editor;

namespace HouraiTeahouse.Localization.Editor {

    /// <summary>
    /// A Editor-Only ScriptableObject for pulling localization data from Google Spreadsheets
    /// and creating the approriate Langauge assets.
    /// </summary>
    [HelpURL("http://wiki.houraiteahouse.net/index.php/Dev:Localization#Localization_Generator")]
    public class LocalizationGenerator : ScriptableObject {

        [MenuItem("Hourai/Localization/Generate")]
        static void Create() {
            var generatorPath = AssetUtil.FindAssetPaths<LocalizationGenerator>();
            LocalizationGenerator generator;
            if (generatorPath.Length <= 0)
                AssetUtil.CreateAssetInProjectWindow<LocalizationGenerator>();
            else {
                generator = AssetDatabase.LoadAssetAtPath<LocalizationGenerator>(generatorPath[0]);
                if(generator)
                    generator.Generate();
            }
        }
        
        [SerializeField, Tooltip("The public Google Spreadsheets link to pull data from")]
        private string GoogleLink;

        [SerializeField, Tooltip("Columns in the spreadsheet to ignore")]
        private string[] _ignoreColumns;

        [SerializeField, Tooltip("The folder to save all of the generated assets into.")]
        private Object _saveFolder;

        /// <summary>
        /// Reads the Google Spreadsheet and generates/updates the Language asset files
        /// </summary>
        public void Generate() {
            ListFeed test = GDocService.GetSpreadsheet(GoogleLink);
            var languageMap = new Dictionary<string, Dictionary<string, string>>();
            HashSet<string> ignore = new HashSet<string>(_ignoreColumns);
            foreach (ListEntry row in test.Entries) {
                foreach (ListEntry.Custom element in row.Elements) {
                    string lang = element.LocalName;
                    if (ignore.Contains(lang))
                        continue;
                    if(!languageMap.ContainsKey(lang))
                        languageMap[lang] = new Dictionary<string, string>();
                    languageMap[lang][row.Title.Text] = element.Value;
                }
            }
            string folderPath = _saveFolder ? AssetDatabase.GetAssetPath(_saveFolder) : "Assets/Resources/Lang";
            foreach (var lang in languageMap) {
                var method = "Generating";
                string path = string.Format("{0}/{1}.asset", folderPath, lang.Key);
                var language = AssetDatabase.LoadAssetAtPath<Language>(path);
                if (language) {
                    method = "Updating";
                    language.ReadFromDictionary(lang.Value);
                    EditorUtility.SetDirty(language);
               }
                else
                    AssetDatabase.CreateAsset(Language.FromDictionary(lang.Value), path);
                Debug.Log(string.Format("{0} language files for: {1}", method, lang.Key));
            }
            EditorApplication.SaveAssets();
            AssetDatabase.SaveAssets();
        }

    }

}
