using HouraiTeahouse.Localization;
using HouraiTeahouse.FantasyCrescendo.Characters;
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
      Debug.Log("Updating localization data...");
      LocalizationGenerator.GenerateAll();
      Debug.Log("Localization data updated!");
#else
  public static void Prebuild() {
      Debug.Log("Starting pre-build cleanup...");
#endif
      Debug.Log("Building asset bundles.");
      // BuildScript.BuildAssetBundles();
      Debug.Log("Finished cleanup.");
  }

}

}
