using HouraiTeahouse.SmashBrew;
using HouraiTeahouse.SmashBrew.Characters;
using HouraiTeahouse.AssetBundles.Editor;
using HouraiTeahouse.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor; 

namespace HouraiTeahouse.FantasyCrescendo {

    public static class BuildCommands {

        [MenuItem("Hourai Teahouse/Clear Character Materials")]
        static void ClearCharacterMaterials() {
            Log.Info("Clearing character materials.");
            var characters = Assets.LoadAll<CharacterData>().Select(c => c.Prefab).Distinct();
            foreach(var character in characters) {
                var prefab = character.Load();
                if (prefab == null)
                    continue;
                var colorStates = prefab.GetComponentsInChildren<ColorState>();
                foreach (var colorState in colorStates)
                    colorState.ClearRenderers();
                EditorUtility.SetDirty(prefab);
                Log.Info("Cleared materials for {0}", prefab.name);
            }
            AssetDatabase.SaveAssets();
            Log.Info("Finished clearing ");
        }

#if UNITY_CLOUD_BUILD
        public static void Prebuild(UnityEngine.CloudBuild.BuildManifestObject manifest) {
            Log.Info("Starting pre-export changes and cleanup...");
            PlayerSettings.bundleVersion += " {0} Build #{1}".With(
                manifest.GetValue<string>("cloudBuildTargetName"), 
                manifest.GetValue<string>("buildNumber"));
            Log.Info("Changed version to {0}", PlayerSettings.bundleVersion);
#else
        public static void Prebuild() {
            Log.Info("Starting pre-build cleanup...");
#endif
            ClearCharacterMaterials();
            Log.Info("Building asset bundles.");
            BuildScript.BuildAssetBundles();
            Log.Info("Finished cleanup.");
        }

    }
}
