using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace HouraiTeahouse.AssetBundles.Editor {

	public class BuildScript {

		public static string OverloadedDevelopmentServerUrl = "";
	
		public static void BuildAssetBundles() {
			// Choose the output path according to the build target.
			string outputPath = Path.Combine(BundleUtility.AssetBundlesOutputPath,  BundleUtility.GetPlatformName());
			if (!Directory.Exists(outputPath))
				Directory.CreateDirectory (outputPath);
	
			//@TODO: use append hash... (Make sure pipeline works correctly with it.)
			BuildPipeline.BuildAssetBundles (outputPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
		}
	
		public static void WriteServerURL() {
			string downloadUrl;
			if (string.IsNullOrEmpty(OverloadedDevelopmentServerUrl) == false)
				downloadUrl = OverloadedDevelopmentServerUrl;
			else {
			    string localIp = "";
				IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
				foreach (IPAddress ip in host.AddressList) {
					if (ip.AddressFamily == AddressFamily.InterNetwork) {
						localIp = ip.ToString();
						break;
					}
				}
				downloadUrl = "http://" + localIp + ":7888/";
			}
			
			const string assetBundleManagerResourcesDirectory = "Assets/AssetBundleManager/Resources";
			string assetBundleUrlPath = Path.Combine (assetBundleManagerResourcesDirectory, "AssetBundleServerURL.bytes");
			Directory.CreateDirectory(assetBundleManagerResourcesDirectory);
			File.WriteAllText(assetBundleUrlPath, downloadUrl);
			AssetDatabase.Refresh();
		}
		
		public static void BuildPlayer() {
			var outputPath = EditorUtility.SaveFolderPanel("Choose Location of the Built Game", "", "");
			if (outputPath.Length == 0)
				return;
			
			string[] levels = GetLevelsFromBuildSettings();
			if (levels.Length == 0) {
				Log.Info("Nothing to build.");
				return;
			}
			
			string targetName = GetBuildTargetName(EditorUserBuildSettings.activeBuildTarget);
			if (targetName == null)
				return;
			
			// Build and copy AssetBundles.
			BuildAssetBundles();
			CopyAssetBundlesTo(Path.Combine(Application.streamingAssetsPath, BundleUtility.AssetBundlesOutputPath));
			AssetDatabase.Refresh();
			
			BuildOptions option = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
			BuildPipeline.BuildPlayer(levels, outputPath + targetName, EditorUserBuildSettings.activeBuildTarget, option);
            if (Directory.Exists(outputPath)) {
                foreach (var file in Directory.GetFiles(outputPath, "*.manifest", SearchOption.AllDirectories))
                    File.Delete(file);
            }
		}
	
		public static string GetBuildTargetName(BuildTarget target) {
		    var baseName =
		        string.Join(string.Empty,
		            PlayerSettings.productName.Split(' ')
		                .Where(s => !s.IsNullOrEmpty())
		                .Select(s => s.Substring(0, 1).ToLower())
                        .ToArray());
			switch(target) {
                case BuildTarget.Android :
			        return "/{0}.apk".With(baseName);
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
			        return "/{0}.exe".With(baseName);
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
			        return "/{0}.app".With(baseName);
                case BuildTarget.WebGL:
                    return "";
                    // Add more build targets for your own.
                default:
                    Log.Error("Target not implemented.");
                    return null;
			}
		}
	
		public static void CopyAssetBundlesTo(string outputPath) {
			// Clear streaming assets folder.
			Directory.CreateDirectory(outputPath);
	
			string outputFolder = BundleUtility.GetPlatformName();
	
			// Setup the source folder for assetbundles.
			var source = Path.Combine(Path.Combine(System.Environment.CurrentDirectory, BundleUtility.AssetBundlesOutputPath), outputFolder);
			if (!Directory.Exists(source))
				Log.Info("No assetBundle output folder, try to build the assetBundles first.");
	
			// Setup the destination folder for assetbundles.
			var destination = Path.Combine(outputPath, outputFolder);
			if (Directory.Exists(destination))
				FileUtil.DeleteFileOrDirectory(destination);
			
			FileUtil.CopyFileOrDirectory(source, destination);
		}
	
		static string[] GetLevelsFromBuildSettings() {
		    return (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray();
		}
	}
}
