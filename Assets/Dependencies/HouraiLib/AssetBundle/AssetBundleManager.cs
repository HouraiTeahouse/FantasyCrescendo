using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR	
using UnityEditor;
#endif
using Object = UnityEngine.Object;

/*
 	In this demo, we demonstrate:
	1.	Automatic asset bundle dependency resolving & loading.
		It shows how to use the manifest assetbundle like how to get the dependencies etc.
	2.	Automatic unloading of asset bundles (When an asset bundle or a dependency thereof is no longer needed, the asset bundle is unloaded)
	3.	Editor simulation. A bool defines if we load asset bundles from the project or are actually using asset bundles(doesn't work with assetbundle variants for now.)
		With this, you can player in editor mode without actually building the assetBundles.
	4.	Optional setup where to download all asset bundles
	5.	Build pipeline build postprocessor, integration so that building a player builds the asset bundles and puts them into the player data (Default implmenetation for loading assetbundles from disk on any platform)
	6.	Use WWW.LoadFromCacheOrDownload and feed 128 bit hash to it when downloading via web
		You can get the hash from the manifest assetbundle.
	7.	AssetBundle variants. A prioritized list of variants that should be used if the asset bundle with that variant exists, first variant in the list is the most preferred etc.
*/

namespace HouraiTeahouse.AssetBundles {	

	// Loaded assetBundle contains the references count which can be used to unload dependent assetBundles automatically.
	public class LoadedAssetBundle {
		public AssetBundle AssetBundle { get; private set; }
		public int ReferencedCount { get; internal set; }
		
		public LoadedAssetBundle(AssetBundle assetBundle) {
			AssetBundle = assetBundle;
			ReferencedCount = 1;
		}
	}

    enum AssetBundleAction {
        Download, Delete
    }

    struct AssetBundleChange {
        public string AssetBundleName { get; set; }
        public AssetBundleAction Action { get; set; }
        public Hash128 Hash { get; set; }
    }

    // Class takes care of loading assetBundle and its dependencies automatically, loading variants automatically.
    public class AssetBundleManager : MonoBehaviour {

	#if UNITY_EDITOR	
		static int _simulateAssetBundleInEditor = -1;
		const string SimulateAssetBundles = "SimulateAssetBundles";
	#endif
	
		static readonly Dictionary<string, LoadedAssetBundle> LoadedAssetBundles = new Dictionary<string, LoadedAssetBundle> ();
		static readonly Dictionary<string, WWW> DownloadingWwWs = new Dictionary<string, WWW> ();
		static readonly Dictionary<string, string> DownloadingErrors = new Dictionary<string, string> ();
		static readonly List<AssetBundleOperation> InProgressOperations = new List<AssetBundleOperation> ();
		static readonly Dictionary<string, string[]> Dependencies = new Dictionary<string, string[]> ();

        static readonly Dictionary<Type, Delegate> TypeHandlers = new Dictionary<Type, Delegate> ();

        public static void AddHandler<T>(Action<T> handler) {
            var type = typeof(T);
            Delegate current;
            TypeHandlers[type] = TypeHandlers.TryGetValue(type, out current) ? Delegate.Combine(current, handler) : handler;
        }

        public static void RemoveHandler<T>(Action<T> handler) {
            TypeHandlers[typeof(T)] = Delegate.RemoveAll(TypeHandlers[typeof(T)], handler);
        }

        public static void LoadLocalBundles(IEnumerable<string> searchPatterns) {
            foreach (var pattern in searchPatterns) {
                var files = Directory.GetFiles(Application.streamingAssetsPath, pattern);
                foreach (var file in files) {
                    //TODO(james7132): Make this asynchronous
                    var bundle = AssetBundle.LoadFromFile(file);
                    if (bundle == null) {
                        Log.Error("Failed to load an AssetBundle from {0}.", file);
                        continue;
                    }
                    var mainAsset = bundle.mainAsset;
                    var assetType = mainAsset.GetType();
                    Delegate handler;
                    if (TypeHandlers.TryGetValue(assetType, out handler)) {
                        handler.DynamicInvoke(mainAsset);
                        Log.Info("Loaded {0} ({1}) from {2}.", mainAsset, assetType, file);
                    } else {
                        Log.Error("Attempted to load an asset of type {0} from asset bundle {1}, but no handler was found. Unloading...");
                        bundle.Unload(true);
                    }
                }
            }
        }

        IEnumerable<AssetBundleChange> DiffManifests(AssetBundleManifest current, AssetBundleManifest remote) {
            var currentBundles = current.GetAllAssetBundles();
            var remoteBundles = remote.GetAllAssetBundles();
            foreach (var bundle in currentBundles.Except(remoteBundles)) {
                yield return new AssetBundleChange {
                    AssetBundleName = bundle,
                    Action = AssetBundleAction.Delete
                };
            }
            foreach (var bundle in remoteBundles.Except(currentBundles)) {
                yield return new AssetBundleChange {
                    AssetBundleName = bundle,
                    Action = AssetBundleAction.Download,
                    Hash = remote.GetAssetBundleHash(bundle)
                };
            }
            foreach (var bundle in currentBundles.Intersect(remoteBundles)) {
                if (current.GetAssetBundleHash(bundle) == remote.GetAssetBundleHash(bundle))
                    continue;
                yield return new AssetBundleChange {
                    AssetBundleName = bundle,
                    Action = AssetBundleAction.Download,
                    Hash = remote.GetAssetBundleHash(bundle)
                };
            }
        }


        static AssetBundleManager() {
	        BaseDownloadingUrl = "";
		    ActiveVariants = new string[0];
	    }
	
		// The base downloading url which is used to generate the full downloading url with the assetBundle names.
		public static string BaseDownloadingUrl { get; set; }
	
		// Variants which is used to define the active variants.
		public static string[] ActiveVariants { get; set; }
	
		// AssetBundleManifest object which can be used to load the dependecies and check suitable assetBundle variants.
		public static AssetBundleManifest Manifest { get; internal set; }
	
	#if UNITY_EDITOR
		// Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
		public static bool SimulateAssetBundleInEditor {
			get {
				if (_simulateAssetBundleInEditor == -1)
					_simulateAssetBundleInEditor = EditorPrefs.GetBool(SimulateAssetBundles, true) ? 1 : 0;
				
				return _simulateAssetBundleInEditor != 0;
			}
			set {
				int newValue = value ? 1 : 0;
				if (newValue != _simulateAssetBundleInEditor) {
					_simulateAssetBundleInEditor = newValue;
					EditorPrefs.SetBool(SimulateAssetBundles, value);
				}
			}
		}
    #endif

	    static string GetStreamingAssetsPath() {
	        if (Application.isEditor)
				return "file://" +  Environment.CurrentDirectory.Replace("\\", "/"); // Use the build output folder directly.
	        if (Application.isWebPlayer)
	            return Path.GetDirectoryName(Application.absoluteURL).Replace("\\", "/")+ "/StreamingAssets";
	        if (Application.isMobilePlatform || Application.isConsolePlatform)
	            return Application.streamingAssetsPath;
	        return "file://" +  Application.streamingAssetsPath;
	    }

	    public static void SetSourceAssetBundleDirectory(string relativePath) {
			BaseDownloadingUrl = GetStreamingAssetsPath() + relativePath;
		}
		
		public static void SetSourceAssetBundleUrl(string absolutePath) {
			BaseDownloadingUrl = absolutePath + Utility.GetPlatformName() + "/";
		}
	
		public static void SetDevelopmentAssetBundleServer() {
			#if UNITY_EDITOR
			// If we're in Editor simulation mode, we don't have to setup a download URL
			if (SimulateAssetBundleInEditor)
				return;
			#endif
			
			var urlFile = Resources.Load("AssetBundleServerURL") as TextAsset;
			string url = (urlFile != null) ? urlFile.text.Trim() : null;
			if (string.IsNullOrEmpty(url))
				Log.Error("Development Server URL could not be found.");
			else
				SetSourceAssetBundleUrl(url);
		}
		
		// Get loaded AssetBundle, only return vaild object when all the dependencies are downloaded successfully.
		public static LoadedAssetBundle GetLoadedAssetBundle (string assetBundleName, out string error) {
			if (DownloadingErrors.TryGetValue(assetBundleName, out error) )
				return null;
		
			LoadedAssetBundle bundle;
			LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
			if (bundle == null)
				return null;
			
			// No dependencies are recorded, only the bundle itself is required.
			string[] dependencies;
			if (!Dependencies.TryGetValue(assetBundleName, out dependencies) )
				return bundle;
			
			// Make sure all dependencies are loaded
			foreach(var dependency in dependencies) {
				if (DownloadingErrors.TryGetValue(assetBundleName, out error) )
					return bundle;
	
				// Wait all the dependent assetBundles being loaded.
				LoadedAssetBundle dependentBundle;
				LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
				if (dependentBundle == null)
					return null;
			}
	
			return bundle;
		}
	
		public static AssetBundleManifestOperation Initialize () {
			return Initialize(Utility.GetPlatformName());
		}
	
		// Load AssetBundleManifest.
		public static AssetBundleManifestOperation Initialize (string manifestAssetBundleName) {
	#if UNITY_EDITOR
			Log.Info("Simulation Mode: " + (SimulateAssetBundleInEditor ? "Enabled" : "Disabled"));
	#endif
	
			var go = new GameObject("AssetBundleManager", typeof(AssetBundleManager));
			DontDestroyOnLoad(go);
		
	#if UNITY_EDITOR	
			// If we're in Editor simulation mode, we don't need the manifest assetBundle.
			if (SimulateAssetBundleInEditor)
				return null;
	#endif
	
			LoadAssetBundle(manifestAssetBundleName, true);
			var operation = new AssetBundleManifestOperation (manifestAssetBundleName, "AssetBundleManifest");
			InProgressOperations.Add (operation);
			return operation;
		}
		
		// Load AssetBundle and its dependencies.
		protected static void LoadAssetBundle(string assetBundleName, bool isLoadingAssetBundleManifest = false) {
			Log.Info("Loading Asset Bundle " + (isLoadingAssetBundleManifest ? "Manifest: " : ": ") + assetBundleName);
	
	#if UNITY_EDITOR
			// If we're in Editor simulation mode, we don't have to really load the assetBundle and its dependencies.
			if (SimulateAssetBundleInEditor)
				return;
	#endif
	
			if (!isLoadingAssetBundleManifest && Manifest == null) {
                Log.Error("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
                return;
			}
	
			// Check if the assetBundle has already been processed.
			bool isAlreadyProcessed = LoadAssetBundleInternal(assetBundleName, isLoadingAssetBundleManifest);
	
			// Load dependencies.
			if (!isAlreadyProcessed && !isLoadingAssetBundleManifest)
				LoadDependencies(assetBundleName);
		}
		
		// Remaps the asset bundle name to the best fitting asset bundle variant.
		protected static string RemapVariantName(string assetBundleName) {
			string[] bundlesWithVariant = Manifest.GetAllAssetBundlesWithVariant();
			string[] split = assetBundleName.Split('.');

			int bestFit = int.MaxValue;
			int bestFitIndex = -1;
			// Loop all the assetBundles with variant to find the best fit variant assetBundle.
			for (var i = 0; i < bundlesWithVariant.Length; i++) {
				string[] curSplit = bundlesWithVariant[i].Split('.');
				if (curSplit[0] != split[0])
					continue;
				
				int found = Array.IndexOf(ActiveVariants, curSplit[1]);
				
				// If there is no active variant found. We still want to use the first 
				if (found == -1)
					found = int.MaxValue-1;
						
				if (found < bestFit) {
					bestFit = found;
					bestFitIndex = i;
				}
			}
			
			if (bestFit == int.MaxValue - 1)
				Log.Warning("Ambigious asset bundle variant chosen because there was no matching active variant: " + bundlesWithVariant[bestFitIndex]);
			
			if (bestFitIndex != -1)
				return bundlesWithVariant[bestFitIndex];
		    return assetBundleName;
		}
	
		// Where we actuall call WWW to download the assetBundle.
		protected static bool LoadAssetBundleInternal (string assetBundleName, bool isLoadingAssetBundleManifest) {
			// Already loaded.
			LoadedAssetBundle bundle;
			LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
			if (bundle != null) {
				bundle.ReferencedCount++;
				return true;
			}
	
			// @TODO: Do we need to consider the referenced count of WWWs?
			// In the demo, we never have duplicate WWWs as we wait LoadAssetAsync()/LoadLevelAsync() to be finished before calling another LoadAssetAsync()/LoadLevelAsync().
			// But in the real case, users can call LoadAssetAsync()/LoadLevelAsync() several times then wait them to be finished which might have duplicate WWWs.
			if (DownloadingWwWs.ContainsKey(assetBundleName) )
				return true;
	
			WWW download = null;
			string url = BaseDownloadingUrl + assetBundleName;
		
			// For manifest assetbundle, always download it as we don't have hash for it.
            //TODO(james7132): Replace this with UnityWebRequest
			if (isLoadingAssetBundleManifest)
				download = new WWW(url);
			else
				download = WWW.LoadFromCacheOrDownload(url, Manifest.GetAssetBundleHash(assetBundleName), 0); 
	
			DownloadingWwWs.Add(assetBundleName, download);
	
			return false;
		}
	
		// Where we get all the dependencies and load them all.
		protected static void LoadDependencies(string assetBundleName) {
			if (Manifest == null) {
				Log.Error("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
				return;
			}
	
			// Get dependecies from the AssetBundleManifest object..
			string[] dependencies = Manifest.GetAllDependencies(assetBundleName);
			if (dependencies.Length == 0)
				return;
				
			for (var i = 0; i < dependencies.Length; i++)
				dependencies[i] = RemapVariantName (dependencies[i]);
				
			// Record and load all dependencies.
			Dependencies.Add(assetBundleName, dependencies);
			foreach (string dependency in dependencies)
			    LoadAssetBundleInternal(dependency, false);
		}
	
		// Unload assetbundle and its dependencies.
		public static void UnloadAssetBundle(string assetBundleName) {
	#if UNITY_EDITOR
			// If we're in Editor simulation mode, we don't have to load the manifest assetBundle.
			if (SimulateAssetBundleInEditor)
				return;
	#endif
			UnloadAssetBundleInternal(assetBundleName);
			UnloadDependencies(assetBundleName);
		}
	
		protected static void UnloadDependencies(string assetBundleName) {
			string[] dependencies;
			if (!Dependencies.TryGetValue(assetBundleName, out dependencies) )
				return;
	
			// Loop dependencies.
			foreach(var dependency in dependencies)
				UnloadAssetBundleInternal(dependency);
	
			Dependencies.Remove(assetBundleName);
		}
	
		protected static void UnloadAssetBundleInternal(string assetBundleName) {
			string error;
			LoadedAssetBundle bundle = GetLoadedAssetBundle(assetBundleName, out error);
			if (bundle == null)
				return;

		    if (--bundle.ReferencedCount != 0)
		        return;
		    bundle.AssetBundle.Unload(false);
		    LoadedAssetBundles.Remove(assetBundleName);
	
		    Log.Info(assetBundleName + " has been unloaded successfully");
		}
	
		void Update()
		{
			// Collect all the finished WWWs.
			var keysToRemove = new List<string>();
			foreach (var keyValue in DownloadingWwWs) {
				WWW download = keyValue.Value;
	
				// If downloading fails.
				if (download.error != null) {
					DownloadingErrors.Add(keyValue.Key, "Failed downloading bundle {0} from {1}: {2}".With(keyValue.Key, download.url, download.error));
					keysToRemove.Add(keyValue.Key);
					continue;
				}
	
				// If downloading succeeds.
				if(download.isDone) {
					AssetBundle bundle = download.assetBundle;
					if (bundle == null) {
					    DownloadingErrors.Add(keyValue.Key, "{0} is not a valid asset bundle.".With(keyValue.Key));
						keysToRemove.Add(keyValue.Key);
						continue;
					}
					LoadedAssetBundles.Add(keyValue.Key, new LoadedAssetBundle(download.assetBundle) );
					keysToRemove.Add(keyValue.Key);
				}
			}
	
			// Remove the finished WWWs.
			foreach( var key in keysToRemove) {
				WWW download = DownloadingWwWs[key];
				DownloadingWwWs.Remove(key);
				download.Dispose();
			}
	
			// Update all in progress operations
			for (var i = 0; i < InProgressOperations.Count;) {
				if (!InProgressOperations[i].Update())
					InProgressOperations.RemoveAt(i);
				else
					i++;
			}
		}
	
		// Load asset from the given assetBundle.
		public static AssetBundleAssetOperation<T> LoadAssetAsync<T>(string assetBundleName, string assetName) where T : Object 
		{
			Log.Info("Loading {0} from {1} bundle...", assetName, assetBundleName);
	
			AssetBundleAssetOperation<T> operation;
	#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor) {
				string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
				if (assetPaths.Length == 0) {
					Log.Error("There is no asset with name \"{0}\" in {1}", assetName, assetBundleName);
					return null;
				}
	
				// @TODO: Now we only get the main object from the first asset. Should consider type also.
			    var target = AssetDatabase.LoadAssetAtPath<T>(assetPaths[0]);
				operation = new AssetBundleAssetOperationSimulation<T>(target);
			}
			else
	#endif
			{
				assetBundleName = RemapVariantName (assetBundleName);
				LoadAssetBundle (assetBundleName);
				operation = new AssetBundleAssetOperationFull<T>(assetBundleName, assetName);
	
				InProgressOperations.Add (operation);
			}
	
			return operation;
		}
	
		// Load level from the given assetBundle.
		public static AssetBundleOperation LoadLevelAsync (string assetBundleName, string levelName, LoadSceneMode loadMode) {
			Log.Info("Loading {0}  from {1} bundle...", levelName, assetBundleName);
	
			AssetBundleOperation operation;
	#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor)
				operation = new AssetBundleLevelSimulationOperation(assetBundleName, levelName, loadMode);
			else
	#endif
			{
				assetBundleName = RemapVariantName(assetBundleName);
				LoadAssetBundle (assetBundleName);
				operation = new AssetBundleLevelOperation (assetBundleName, levelName, loadMode);
	
				InProgressOperations.Add (operation);
			}
	
			return operation;
		}
	} 
}
