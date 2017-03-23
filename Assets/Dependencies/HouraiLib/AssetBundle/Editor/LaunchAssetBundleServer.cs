using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;

namespace HouraiTeahouse.AssetBundles.Editor
{
	internal class LaunchAssetBundleServer : ScriptableSingleton<LaunchAssetBundleServer> {

		const string LocalAssetbundleServerMenu = "Assets/AssetBundles/Local AssetBundle Server";

		[SerializeField]
		int m_ServerPID;

		[MenuItem (LocalAssetbundleServerMenu)]
		public static void ToggleLocalAssetBundleServer () {
			bool isRunning = IsRunning();
			if (!isRunning)
				Run ();
			else
				KillRunningAssetBundleServer ();
		}

		[MenuItem (LocalAssetbundleServerMenu, true)]
		public static bool ToggleLocalAssetBundleServerValidate () {
			bool isRunnning = IsRunning ();
			UnityEditor.Menu.SetChecked(LocalAssetbundleServerMenu, isRunnning);
			return true;
		}

		static bool IsRunning () {
			if (instance.m_ServerPID == 0)
				return false;

			var process = Process.GetProcessById (instance.m_ServerPID);
			if (process == null)
				return false;

			return !process.HasExited;
		}

		static void KillRunningAssetBundleServer () {
			// Kill the last time we ran
			try {
				if (instance.m_ServerPID == 0)
					return;

				var lastProcess = Process.GetProcessById (instance.m_ServerPID);
				lastProcess.Kill();
				instance.m_ServerPID = 0;
			} catch {
			}
		}
		
		static void Run () {
			string pathToAssetServer = Path.Combine(Application.dataPath, "AssetBundleManager/Editor/AssetBundleServer.exe");
			string pathToApp = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
	
			KillRunningAssetBundleServer();
			
			BuildScript.WriteServerURL();
			
			string args = Path.Combine (pathToApp, "AssetBundles");
			args = string.Format("\"{0}\" {1}", args, Process.GetCurrentProcess().Id);
			ProcessStartInfo startInfo = ExecuteInternalMono.GetProfileStartInfoForMono(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), "4.0", pathToAssetServer, args, true);
			startInfo.WorkingDirectory = Path.Combine(System.Environment.CurrentDirectory, "AssetBundles");
			startInfo.UseShellExecute = false;
			Process launchProcess = Process.Start(startInfo);
			if (launchProcess == null || launchProcess.HasExited || launchProcess.Id == 0) {
				//Unable to start process
				UnityEngine.Debug.LogError ("Unable Start AssetBundleServer process");
			} else {
				//We seem to have launched, let's save the PID
				instance.m_ServerPID = launchProcess.Id;
			}
		}
	}
}
