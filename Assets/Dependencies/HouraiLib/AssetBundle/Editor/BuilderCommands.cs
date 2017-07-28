using HouraiTeahouse.SmashBrew;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.AssetBundles.Editor {

    public static class BuilderCommands {

        [MenuItem("Hourai Teahouse/Util/Clear Player Prefs")]
        public static void ClearPlayerPrefs() {
            PlayerPrefs.DeleteAll();
        }

        [MenuItem("Hourai Teahouse/Build/Build Asset Bundles (Windows)")]
        public static void BuildAssetBundlesWindows() {
            BuildScript.BuildAssetBundles(BuildTarget.StandaloneWindows64);
        }

        [MenuItem("Hourai Teahouse/Build/Build Asset Bundles (Mac OSX)")]
        public static void BuildAssetBundlesOSX() {
            BuildScript.BuildAssetBundles(BuildTarget.StandaloneOSXUniversal);
        }

        [MenuItem("Hourai Teahouse/Build/Build Asset Bundles (Linux)")]
        public static void BuildAssetBundlesLinux() {
            BuildScript.BuildAssetBundles(BuildTarget.StandaloneLinuxUniversal);
        }

#if UNITY_CLOUD_BUILD
        public static void BuildCurrentBundles(UnityEngine.CloudBuild.BuildManifestObject manifest) {
            PlayerSettings.bundleVersion += " {0} Build #{1}".With(
                manifest.GetValue<string>("cloudBuildTargetName"), 
                manifest.GetValue<string>("buildNumber"));
#else
        public static void BuildCurrentBundles() {
#endif
            BuildScript.BuildAssetBundles();
        }

    }

}

