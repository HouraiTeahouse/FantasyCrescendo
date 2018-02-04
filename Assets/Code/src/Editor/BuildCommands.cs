using HouraiTeahouse.FantasyCrescendo.Characters;
using HouraiTeahouse.Loadables.AssetBundles;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor; 

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A set of static editor commands to automate or simplify the development process.
/// </summary>
public static class BuildCommands {

  /// <summary>
  /// Clears all the Character prefabs to make sure that they aren't directly referencing
  /// Materials before Asset Bundle Builds.
  /// </summary>
  [MenuItem("Hourai Teahouse/Clear Character Materials")]
  static void ClearCharacterMaterials() {
    Debug.Log("Clearing character materials.");
    var characters = EditorAssetUtil.LoadAll<CharacterData>().Select(c => c.Prefab).Distinct();
    foreach(var character in characters) {
      var prefab = character.Load();
      if (prefab == null) continue;
      var colorStates = prefab.GetComponentsInChildren<CharacterColor>();
      foreach (var color in colorStates) {
        color.Clear();
      }
      EditorUtility.SetDirty(prefab);
      Debug.Log($"Cleared materials for {prefab.name}");
    }
    AssetDatabase.SaveAssets();
    Debug.Log("Finished clearing ");
  }

  /// <summary>
  /// Unity Cloud Build Pre-Build Command.
  /// </summary>
  /// <param name="manifest"></param>
#if UNITY_CLOUD_BUILD
  public static void Prebuild(UnityEngine.CloudBuild.BuildManifestObject manifest) {
      Debug.Log("Starting pre-export changes and cleanup...");
      PlayerSettings.bundleVersion += string.Format(" {0} Build #{1}",
          manifest.GetValue<string>("cloudBuildTargetName"), 
          manifest.GetValue<string>("buildNumber"));
      Debug.Log($"Changed version to {PlayerSettings.bundleVersion}");
#else
  public static void Prebuild() {
      Debug.Log("Starting pre-build cleanup...");
#endif
      ClearCharacterMaterials();
      Debug.Log("Building asset bundles.");
      BuildScript.BuildAssetBundles();
      Debug.Log("Finished cleanup.");
  }

}

}