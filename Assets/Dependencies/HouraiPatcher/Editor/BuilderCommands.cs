using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.HouraiPatcher {

    public static  class BuilderCommandscs {

        [MenuItem("Hourai Teahouse/Build/Build Asset Bundles (Windows)")]
        public static void BuildAssetBundlesWindows() {
            BuildAssetBundles(BuildTarget.StandaloneWindows64);
        }

        [MenuItem("Hourai Teahouse/Build/Build Asset Bundles (Mac OSX)")]
        public static void BuildAssetBundlesOSX() {
            BuildAssetBundles(BuildTarget.StandaloneOSXUniversal);
        }

        [MenuItem("Hourai Teahouse/Build/Build Asset Bundles (Linux)")]
        public static void BuildAssetBundlesLinux() {
            BuildAssetBundles(BuildTarget.StandaloneLinuxUniversal);
        }

        [MenuItem("Hourai Teahouse/Test")]
        public static void Test() {
            Log.Info(Application.streamingAssetsPath);
        }

        static void BuildAssetBundles(BuildTarget target) {
            if (!AssetDatabase.IsValidFolder("Assets/StreamingAssets")) {
                FileUtil.DeleteFileOrDirectory("Assets/StreamingAssets");
                AssetDatabase.CreateFolder("Assets", "StreamingAssets");
            }
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", BuildAssetBundleOptions.None, target);
            AssetDatabase.Refresh();
            Log.Info("Built Asset Bundles for " + target);
        }

    }

}

