using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR	
using UnityEditor;
#endif

namespace HouraiTeahouse.AssetBundles {

	public static class BundleUtility {

		public const string AssetBundlesOutputPath = "bundles";

#if !UNITY_EDITOR
        static bool? writeable;
#endif

        public static string GetStoragePath() {
#if UNITY_EDITOR
            return Application.streamingAssetsPath;
#else
            if (writeable != null) 
                return writeable.Value ? Application.streamingAssetsPath : Application.persistentDataPath;
            try {
                var file = Path.Combine(Application.streamingAssetsPath, "test.txt");
                File.WriteAllText(file, "test");
                File.Delete(file);
                writeable = true;
                return Application.streamingAssetsPath;
            } catch (UnauthorizedAccessException) {
                writeable = false;
                return Application.persistentDataPath;
            }
#endif
        }

        public static string GetBundleStoragePath() {
            return Path.Combine(GetStoragePath(), Path.Combine(AssetBundlesOutputPath, GetPlatformName()));
        }

	    const string AndroidPlatform = "Android";
	    const string iOSPlatform = "iOS";
	    const string WebGLPlatform = "WebGL";
	    const string WindowsPlatform = "Windows";
	    const string MacOSPlatform = "OSX";
	    const string LinuxPlatform = "Linux";
	
#if UNITY_EDITOR
	    public static string GetPlatformName (BuildTarget? target = null) {
			switch(target ?? EditorUserBuildSettings.activeBuildTarget) {
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
#else
	    public static string GetPlatformName (RuntimePlatform? platform = null) {
			switch(platform ?? Application.platform) {
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
#endif
	}
}
