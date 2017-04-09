using UnityEditor;

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

        public static void BuildCurrentBundles() {
            BuildScript.BuildAssetBundles();
        }

    }

}

