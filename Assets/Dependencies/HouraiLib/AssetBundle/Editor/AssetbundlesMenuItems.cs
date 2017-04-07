using System.IO;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.AssetBundles.Editor {

	public class AssetBundlesMenuItems {
		const string SimulationMode = "Assets/AssetBundles/Simulation Mode";
	
		[MenuItem(SimulationMode)]
		public static void ToggleSimulationMode () {
			AssetBundleManager.SimulateAssetBundleInEditor = !AssetBundleManager.SimulateAssetBundleInEditor;
		}
	
		[MenuItem(SimulationMode, true)]
		public static bool ToggleSimulationModeValidate () {
			UnityEditor.Menu.SetChecked(SimulationMode, AssetBundleManager.SimulateAssetBundleInEditor);
			return true;
		}
		
		[MenuItem ("Build/Build AssetBundles")]
		public static void BuildAssetBundles () {
			BuildScript.BuildAssetBundles();
		    var path = Path.Combine(Application.streamingAssetsPath, BundleUtility.AssetBundlesOutputPath);
		    if (Directory.Exists(path))
		        FileUtil.DeleteFileOrDirectory(path);
            BuildScript.CopyAssetBundlesTo(path);
            AssetDatabase.Refresh();
		}
		
		[MenuItem ("Build/Build Player")]
		public static void BuildStandalone () {
			BuildScript.BuildPlayer();
		}

	}

}
