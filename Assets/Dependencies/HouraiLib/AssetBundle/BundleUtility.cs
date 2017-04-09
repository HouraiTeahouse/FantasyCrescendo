using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR	
using UnityEditor;
#endif

namespace HouraiTeahouse.AssetBundles {

	public class BundleUtility {

		public const string AssetBundlesOutputPath = "bundles";

        public static string GetStoragePath() {
#if UNITY_EDITOR
            return Application.streamingAssetsPath;
#else
            try {
                var file = Path.Combine(Application.streamingAssetsPath, "test.txt");
                File.WriteAllText(file, "test");
                File.Delete(file);
                return Application.streamingAssetsPath;
            } catch (UnauthorizedAccessException) {
                return Application.persistentDataPath;
            }
#endif 
        }

        public static string GetBundleStoragePath() {
            return Path.Combine(GetStoragePath(), Path.Combine(AssetBundlesOutputPath, GetPlatformName()));
        }
	
		public static string GetPlatformName() {
#if UNITY_EDITOR
			return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
			return GetPlatformForAssetBundles(Application.platform);
#endif
		}

	    const string AndroidPlatform = "Android";
	    const string iOSPlatform = "iOS";
	    const string WebGLPlatform = "WebGL";
	    const string WindowsPlatform = "Windows";
	    const string MacOSPlatform = "OSX";
	    const string LinuxPlatform = "Linux";
	
#if UNITY_EDITOR
	    static string GetPlatformForAssetBundles(BuildTarget target) {
			switch(target) {
                case BuildTarget.Android:
                    return AndroidPlatform;
                case BuildTarget.iOS:
                    return iOSPlatform;
                case BuildTarget.WebGL:
                    return WebGLPlatform;
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return WindowsPlatform;
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
                    return MacOSPlatform;
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
			        return LinuxPlatform;
                default:
                    return null;
			}
		}
#endif

	    static string GetPlatformForAssetBundles(RuntimePlatform platform) {
			switch(platform) {
                case RuntimePlatform.Android:
                    return AndroidPlatform;
                case RuntimePlatform.IPhonePlayer:
                    return iOSPlatform;
                case RuntimePlatform.WebGLPlayer:
                    return WebGLPlatform;
                case RuntimePlatform.WindowsPlayer:
                    return WindowsPlatform;
                case RuntimePlatform.OSXPlayer:
                    return MacOSPlatform;
                case RuntimePlatform.LinuxPlayer:
			        return LinuxPlatform;
                default:
                    return null;
			}
		}
	}
}
