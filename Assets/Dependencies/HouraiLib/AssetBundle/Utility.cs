#if UNITY_EDITOR	
using UnityEditor;
#endif

namespace HouraiTeahouse.AssetBundles {

	public static class Utility {

		public const string AssetBundlesOutputPath = "AssetBundles";
	
		public static string GetPlatformName() {
	#if UNITY_EDITOR
			return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
	#else
			return GetPlatformForAssetBundles(Application.platform);
	#endif
		}
	
	#if UNITY_EDITOR
	    static string GetPlatformForAssetBundles(BuildTarget target) {
			switch(target) {
                case BuildTarget.Android:
                case BuildTarget.iOS:
                case BuildTarget.WebGL:
                    return target.ToString();
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
                    return "OSX";
                    // Add more build targets for your own.
                    // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                default:
                    return null;
			}
		}
	#endif
	}
}
