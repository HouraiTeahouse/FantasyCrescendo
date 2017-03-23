using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace HouraiTeahouse.AssetBundles {

	public abstract class AssetBundleLoadOperation : AsyncOperation, IEnumerator {

		public object Current {
            get { return null; }
		}

		public bool MoveNext() {
			return !IsDone();
		}
		
		public void Reset()
		{
		}
		
		public abstract bool Update ();
		
		public abstract bool IsDone ();
	}
	
	#if UNITY_EDITOR
	public class AssetBundleLoadLevelSimulationOperation : AssetBundleLoadOperation {

	    readonly AsyncOperation _operation;
	
		public AssetBundleLoadLevelSimulationOperation (string assetBundleName, string levelName, LoadSceneMode loadMode) {
			string[] levelPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, levelName);
			if (levelPaths.Length == 0) {
				///@TODO: The error needs to differentiate that an asset bundle name doesn't exist
				//        from that there right scene does not exist in the asset bundle...
				
				Log.Error("There is no scene with name \"" + levelName + "\" in " + assetBundleName);
				return;
			}

		    _operation = SceneManager.LoadSceneAsync(levelPaths[0], loadMode);
		}
		
		public override bool Update ()
		{
			return false;
		}
		
		public override bool IsDone ()
		{		
			return _operation == null || _operation.isDone;
		}
	}
	
	#endif
	public class AssetBundleLoadLevelOperation : AssetBundleLoadOperation
	{
		protected string AssetBundleName { get; set; }
		protected string LevelName { get; set; }
		protected LoadSceneMode Mode { get; set; }
		protected string DownloadingError { get; set; }
		protected AsyncOperation Request { get; set; }
	
		public AssetBundleLoadLevelOperation (string assetbundleName, string levelName, LoadSceneMode mode = LoadSceneMode.Single) {
			AssetBundleName = assetbundleName;
			LevelName = levelName;
		    Mode = mode;
		}
	
		public override bool Update () {
			if (Request != null)
				return false;

		    string downloadingError;
		    LoadedAssetBundle bundle = AssetBundleManager.GetLoadedAssetBundle (AssetBundleName, out downloadingError);
		    DownloadingError = downloadingError;
		    if (bundle == null)
		        return true;
		    SceneManager.LoadSceneAsync(LevelName, Mode);
		    return false;
		}
		
		public override bool IsDone () {
			// Return if meeting downloading error.
			// m_DownloadingError might come from the dependency downloading.
		    if (Request != null || DownloadingError == null)
		        return Request != null && Request.isDone;
		    Log.Error(DownloadingError);
		    return true;
		}
	}
	
	public abstract class AssetBundleLoadAssetOperation<T> : AssetBundleLoadOperation where T : Object {
		public abstract T Asset { get; }
	}
	
	public class AssetBundleLoadAssetOperationSimulation<T> : AssetBundleLoadAssetOperation<T> where T : Object {
	    readonly Object	_simulatedObject;
		
		public AssetBundleLoadAssetOperationSimulation (Object simulatedObject) {
			_simulatedObject = simulatedObject;
		}
		
		public override T Asset {
            get { return _simulatedObject as T; }
		}
		
		public override bool Update () {
			return false;
		}
		
		public override bool IsDone () {
			return true;
		}
	}
	
	public class AssetBundleLoadAssetOperationFull<T> : AssetBundleLoadAssetOperation<T> where T : Object {

		protected string AssetBundleName { get; set; }
		protected string AssetName { get; set; }
		protected string DownloadingError { get; set; }
		protected AssetBundleRequest Request { get; set; }
	
		public AssetBundleLoadAssetOperationFull (string bundleName, string assetName) {
			AssetBundleName = bundleName;
			AssetName = assetName;
		}
		
		public override T Asset  {
            get {
                if (Request != null && Request.isDone)
                    return Request.asset as T;
                return null;
            }
		}

	    // Returns true if more Update calls are required.
		public override bool Update () {
			if (Request != null)
				return false;

		    string downloadingError;
		    LoadedAssetBundle bundle = AssetBundleManager.GetLoadedAssetBundle (AssetBundleName, out downloadingError);
		    DownloadingError = downloadingError;
			if (bundle != null) {
				///@TODO: When asset bundle download fails this throws an exception...
				Request = bundle.AssetBundle.LoadAssetAsync<T>(AssetName);
				return false;
			}
		    return true;
		}
		
		public override bool IsDone () {
			// Return if meeting downloading error.
			// m_DownloadingError might come from the dependency downloading.
			if (Request == null && DownloadingError != null) {
				Log.Error(DownloadingError);
				return true;
			}
			return Request != null && Request.isDone;
		}
	}
	
	public class AssetBundleLoadManifestOperation : AssetBundleLoadAssetOperationFull<AssetBundleManifest> {

		public AssetBundleLoadManifestOperation (string bundleName, string assetName) : base(bundleName, assetName)
		{
		}
	
		public override bool Update () {
			base.Update();

		    if (Request == null || !Request.isDone)
		        return true;
		    AssetBundleManager.Manifest = Asset;
		    return false;
		}
	}
	
}
