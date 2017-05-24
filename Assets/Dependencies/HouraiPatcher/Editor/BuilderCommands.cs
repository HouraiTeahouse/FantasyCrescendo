using HouraiTeahouse.SmashBrew;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.AssetBundles.Editor {

    public static class BuilderCommands {

        [MenuItem("Hourai Teahouse/jkBuild/Build Asset Bundles (Windows)")]
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
                manifest.GetValue("cloudBuildTargetName"), 
                manifest.GetValue("buildNumber"));
            string branch = manifest.GetValue("scmBranch");
            var config = Config.Instance;
            var serializedConfig = new SerializedObject(config);
            serializedConfig.FindProperty("_bundles._branch").stringValue = branch;
            serializedConfig.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            Log.Info("Set Bundle Branch to \"{0}\"".With(branch));
            Log.Info("Base URL set to \"{0}\"".With(BundleUtility.GetRemoteBundleUri("")));
#else
        public static void BuildCurrentBundles() {
#endif
            BuildScript.BuildAssetBundles();
        }

    }

}

