using System.Collections.Generic;
using UnityEngine;
using Hourai.Editor;
using UnityEditor;

using Google.GData.Client;
using Google.GData.Spreadsheets;

namespace Hourai.Localization.Editor {

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
        
        [SerializeField]
        private string GoogleLink;

        [SerializeField]
        private int keyCol = 1;

        [SerializeField]
        private int[] _ignoreColumns;

        public void Generate() {
            ListFeed test = GDocService.GetSpreadsheet(GoogleLink);
            var languageMap = new Dictionary<string, Dictionary<string, string>>();
            foreach (ListEntry row in test.Entries) {
                foreach (var lang in languageMap) {
                    
                }
                foreach (ListEntry.Custom element in row.Elements) {
                    Debug.Log(element.LocalName);
                }
            }
        }

    }

}