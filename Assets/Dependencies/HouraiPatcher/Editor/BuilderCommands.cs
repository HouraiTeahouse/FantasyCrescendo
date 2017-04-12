using HouraiTeahouse.SmashBrew;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.AssetBundles.Editor {

    public static class BuilderCommands {

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
            string branch = manifest.GetValue("scmBranch");
#else
        public static void BuildCurrentBundles() {
            const string branch = "master";
#endif
            var config = Config.Instance;
            var serializedConfig = new SerializedObject(config);
            serializedConfig.FindProperty("_bundles._branch").stringValue = branch;
            serializedConfig.ApplyModifiedProperties();
            Log.Info("Set Bundle Branch to \"{0}\"".With(Config.Bundles.Branch));
            Log.Info("Base URL set to \"{0}\"".With(Config.Bundles.GetBundleUrl("")));
            BuildScript.BuildAssetBundles();
        }

    }

}

