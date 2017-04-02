using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR	
using UnityEditor;
#endif
using Object = UnityEngine.Object;

namespace HouraiTeahouse.AssetBundles {	

	// Loaded assetBundle contains the references count which can be used to unload dependent assetBundles automatically.
	public class LoadedAssetBundle {
        public string Name { get; private set; }
		public AssetBundle AssetBundle { get; private set; }
		public int ReferencedCount { get; internal set; }
		
		public LoadedAssetBundle(string name, AssetBundle assetBundle) {
		    Name = name;
			AssetBundle = assetBundle;
			ReferencedCount = 1;
		}
	}

    // Class takes care of loading assetBundle and its dependencies automatically, loading variants automatically.
    public static class AssetBundleManager {

	#if UNITY_EDITOR	
		static int _simulateAssetBundleInEditor = -1;
		const string SimulateAssetBundles = "SimulateAssetBundles";
	#endif

        static readonly Regex SeperatorRegex = new Regex(@"[\\]", RegexOptions.Compiled);
        static readonly ILog log = Log.GetLogger("AssetBundle");
        public static readonly ITask<AssetBundleManifest> Manifest = new Task<AssetBundleManifest>();
		static readonly Dictionary<string, ITask<LoadedAssetBundle>> AssetBundles = new Dictionary<string, ITask<LoadedAssetBundle>> ();
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

        public static void CopyDirectory(string src, string dest) {
            if (src == dest)
                return;
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(src, "*", SearchOption.AllDirectories)) {
                var dir = dirPath.Replace(src, dest);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(src, "*", SearchOption.AllDirectories)) {
                var file = newPath.Replace(src, dest);
                if (!File.Exists(file))
                    File.Copy(newPath, file, true);
            }
        }

        public static ITask LoadLocalBundles(IEnumerable<string> whitelist, IEnumerable<string> blacklist = null) {
            if (whitelist.IsNullOrEmpty())
                return Task.Resolved;
            log.Info("Loading local asset bundles from {0}....", BundleUtility.GetBundleStoragePath());
            string basePath = BundleUtility.GetBundleStoragePath() + Path.DirectorySeparatorChar;
            IEnumerable<string> files;
            try {
                files = Directory.GetFiles(basePath, "*", SearchOption.AllDirectories);
            } catch (DirectoryNotFoundException) {
                log.Error("Base directory {0} cannot be found. Cannot load local bundles.", basePath);
                return Task.Resolved;
            }
            var whitelistRegex = whitelist.EmptyIfNull()
                .Select(r => new Regex(r.Replace("/", Regex.Escape(Path.DirectorySeparatorChar.ToString()))
                .Replace("*", "(.*?)"), 
                RegexOptions.Compiled)).ToArray();
            var blacklistRegex = blacklist.EmptyIfNull()
                .Select(r => new Regex(r.Replace("/", Regex.Escape(Path.DirectorySeparatorChar.ToString()))
                .Replace("*", "(.*?)"), 
                RegexOptions.Compiled)).ToArray();
            var bundles = new List<string>();
            foreach (var file in files) {
                var filePath = file.Replace(basePath, string.Empty);
                if (whitelistRegex.All(r => !r.IsMatch(filePath)) || blacklistRegex.Any(r => r.IsMatch(filePath)))
                    continue;
#if UNITY_EDITOR
                if (file.EndsWith(".meta"))
                    continue;
#endif
                bundles.Add(filePath);
            }
            var bundleTasks = bundles.Select(name => LoadAssetBundleAsync(name).Then(loadedBundle => {
                var bundle = loadedBundle.AssetBundle;
                if (bundle == null) {
                    log.Error("Failed to load an AssetBundle from {0}.", loadedBundle.Name);
                    return Task.Resolved;
                }
                var assetName = bundle.GetAllAssetNames()[0];
                ITask<Object> assetTask = bundle.mainAsset != null 
                    ? Task.FromResult(bundle.mainAsset) 
                    : LoadAssetAsync<Object>(loadedBundle.Name, assetName);
                return assetTask.Then(asset => {
                    if (asset == null)
                        return;
                    var assetType = asset.GetType();
                    Delegate handler;
                    if (TypeHandlers.TryGetValue(assetType, out handler)) {
                        log.Info("Loaded {0} ({1}) from \"{2}\".", asset.name, assetType.Name, loadedBundle.Name);
                        handler.DynamicInvoke(asset);
                    } else {
                        log.Error(
                            "Attempted to load an asset of type {0} from asset bundle {1}, but no handler was found. Unloading...",
                            assetType,
                            loadedBundle.Name);
                        UnloadAssetBundle(loadedBundle.Name);
                    }
                });
            }));
            var task = Task.All(bundleTasks);
            task.Then(() => log.Info("Done loading local asset bundles"));
            return task;
        }

        static AssetBundleManager() {
	        BaseDownloadingUrl = "";
		    ActiveVariants = new string[0];
	    }
	
		// The base downloading url which is used to generate the full downloading url with the assetBundle names.
		public static string BaseDownloadingUrl { get; set; }
	
		// Variants which is used to define the active variants.
		public static string[] ActiveVariants { get; set; }
	
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
			    if (newValue == _simulateAssetBundleInEditor)
			        return;
			    _simulateAssetBundleInEditor = newValue;
			    EditorPrefs.SetBool(SimulateAssetBundles, value);
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
			BaseDownloadingUrl = absolutePath + BundleUtility.GetPlatformName() + "/";
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
				log.Error("Development Server URL could not be found.");
			else
				SetSourceAssetBundleUrl(url);
		}
	
		public static ITask<AssetBundleManifest> Initialize () {
			return Initialize(BundleUtility.GetPlatformName());
		}
	
		// Load AssetBundleManifest.
		public static ITask<AssetBundleManifest> Initialize (string manifestAssetBundleName) {
	#if UNITY_EDITOR	
			log.Info("Simulation Mode: " + (SimulateAssetBundleInEditor ? "Enabled" : "Disabled"));
			// If we're in Editor simulation mode, we don't need the manifest assetBundle.
			if (SimulateAssetBundleInEditor)
				return Task.FromResult<AssetBundleManifest>(null);
	#endif

		    var task = LoadAssetAsync<AssetBundleManifest>(manifestAssetBundleName, "AssetBundleManifest");
            task.Then(manifest => Manifest.Resolve(manifest));
			return task;
		}
		
		// Load AssetBundle and its dependencies.
		static ITask<LoadedAssetBundle> LoadAssetBundleAsync(string assetBundleName, bool isLoadingAssetBundleManifest = false) {
			log.Info("Loading Asset Bundle" + (isLoadingAssetBundleManifest ? " Manifest: " : ": ") + assetBundleName);
	
	#if UNITY_EDITOR
			// If we're in Editor simulation mode, we don't have to really load the assetBundle and its dependencies.
			if (SimulateAssetBundleInEditor)
				return Task.FromResult<LoadedAssetBundle>(null);
	#endif

		    var mainTask = LoadAssetBundleInternal(assetBundleName);
		    if (isLoadingAssetBundleManifest)
		        return mainTask;
		    return LoadDependencies(assetBundleName).Then(deps => mainTask);
		}
		
		// Remaps the asset bundle name to the best fitting asset bundle variant.
		static ITask<string> RemapVariantName(string assetBundleName) {
		    return Manifest.Then(manifest => {
		        string[] bundlesWithVariant = manifest.GetAllAssetBundlesWithVariant();
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
		                found = int.MaxValue - 1;

		            if (found < bestFit) {
		                bestFit = found;
		                bestFitIndex = i;
		            }
		        }

		        if (bestFit == int.MaxValue - 1)
		            log.Warning("Ambigious asset bundle variant chosen because there was no matching active variant: "
		                + bundlesWithVariant[bestFitIndex]);

		        if (bestFitIndex != -1)
		            return bundlesWithVariant[bestFitIndex];
		        return assetBundleName;
		    });
		}
	
		// Where we actually load the assetbundles from the local disk.
        static ITask<LoadedAssetBundle> LoadAssetBundleInternal(string assetBundleName) {
            // Already loaded.
            var name = SeperatorRegex.Replace(assetBundleName, "/");
            ITask<LoadedAssetBundle> bundle;
            if (AssetBundles.TryGetValue(name, out bundle)) {
                bundle.Then(b => b.ReferencedCount++);
                return bundle;
            }

            var filePath = Path.Combine(BundleUtility.GetBundleStoragePath(), name);

            // For manifest assetbundle, always download it as we don't have hash for it.
            if (!File.Exists(filePath))
                return Task.FromError<LoadedAssetBundle>(new FileNotFoundException("No file found at {0}".With(filePath)));
            var operation = AssetBundle.LoadFromFileAsync(filePath);
            var task = AsyncManager.AddOperation(operation).Then(request => {
                var assetBundle = request.assetBundle;
                if (assetBundle == null) 
                    throw new Exception("{0} is not a valid asset bundle.".With(name));
                var loadedBundle = new LoadedAssetBundle(name, assetBundle);
                return loadedBundle;
            });
            task.Then(() => { log.Info("Loaded bundle \"{0}\" from {1}.", name, filePath); });
            AssetBundles.Add(name, task);
            return task;
        }
	
		// Where we get all the dependencies and load them all.
		static ITask<LoadedAssetBundle[]> LoadDependencies(string assetBundleName) {
		    return Manifest.Then(manifest => {
                // Get dependecies from the AssetBundleManifest object..
		        var dependencies = Task.All(manifest.GetAllDependencies(assetBundleName).Select(RemapVariantName));

		        // Record and load all dependencies.
		        dependencies.Then(deps => Dependencies.Add(assetBundleName, deps));
		        return dependencies.Then(deps => Task.All(deps.Select(LoadAssetBundleInternal)));
		    });
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
	
		static void UnloadDependencies(string assetBundleName) {
			string[] dependencies;
			if (!Dependencies.TryGetValue(assetBundleName, out dependencies))
				return;
	
			// Loop dependencies.
			foreach(var dependency in dependencies)
				UnloadAssetBundleInternal(dependency);
	
			Dependencies.Remove(assetBundleName);
		}
	
		static void UnloadAssetBundleInternal(string assetBundleName) {
		    ITask<LoadedAssetBundle> task;
		    if (!AssetBundles.TryGetValue(assetBundleName, out task))
		        return;
		    task.Then(bundle => {
		        if (bundle == null || --bundle.ReferencedCount != 0)
		            return;
		        bundle.AssetBundle.Unload(false);
		        AssetBundles.Remove(assetBundleName);
		        log.Info("{0} has been unloaded successfully", assetBundleName);
		    });
		}

		// Loads an asset given a bundle encoded path
        public static ITask<T> LoadAssetAsync<T>(string assetPath) where T : Object {
            if (assetPath.IndexOf(Resource.BundleSeperator) < 0)
                return Task.FromError<T>(new ArgumentException(
                    "assetPath must contain the bundle seperator ({0}) to load from asset bundles".With(Resource.BundleSeperator)
                    ));
            string[] parts = assetPath.Split(Resource.BundleSeperator);
            return LoadAssetAsync<T>(parts[0], parts[1]);
        }
	
		// Load asset from the given assetBundle.
		public static ITask<T> LoadAssetAsync<T>(string assetBundleName, string assetName) where T : Object {
			log.Info("Loading {0} from {1} bundle...", assetName, assetBundleName);
	
	#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor) {
				string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
			    if (assetPaths.Length != 0)
			        return Task.FromResult(AssetDatabase.LoadAssetAtPath<T>(assetPaths[0]));
			    var message = "There is no asset with name \"{0}\" in {1}".With(assetName, assetBundleName);
			    log.Error(message);
			    return Task.FromError<T>(new Exception(message));
			}
	#endif
		    ITask<LoadedAssetBundle> task;
		    if (typeof(T) == typeof(AssetBundleManifest))
		        task = LoadAssetBundleInternal(assetBundleName);
            else
                task = RemapVariantName(assetBundleName).Then(bundleName => LoadAssetBundleAsync(bundleName));
            var assetTask = task.Then(bundle => AsyncManager.AddOperation(bundle.AssetBundle.LoadAssetAsync<T>(assetName)));
            assetTask.Then(() => log.Info("Loaded {0} from {1}", assetName, assetBundleName));
            return assetTask.Then(request => request.asset as T);
		}

		// Loads a scene given a bundle encoded path
        public static ITask LoadLevelAsync(string assetPath,
                                           LoadSceneMode loadMode = LoadSceneMode.Single) {
            if (assetPath.IndexOf(Resource.BundleSeperator) < 0)
                return Task.FromError(new ArgumentException(
                    "assetPath must contain the bundle seperator ({0}) to load from asset bundles".With(Resource.BundleSeperator)
                    ));
            string[] parts = assetPath.Split(Resource.BundleSeperator);
            return LoadLevelAsync(parts[0], parts[1], loadMode);
        }

        // Load level from the given assetBundle.
		public static ITask LoadLevelAsync (string assetBundleName, string levelName, LoadSceneMode loadMode = LoadSceneMode.Single) {
			log.Info("Loading {0} from {1} bundle...", levelName, assetBundleName);

#if UNITY_EDITOR
		    if (SimulateAssetBundleInEditor) {
                string[] levelPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, levelName);
                if (levelPaths.Length == 0) {
                    //TODO: The error needs to differentiate that an asset bundle name doesn't exist from that there right scene does not exist in the asset bundle...
                    return Task.FromError(new Exception("There is no scene with name \"" + levelName + "\" in " + assetBundleName));
                }
		        return AsyncManager.AddOperation(SceneManager.LoadSceneAsync(levelPaths[0], loadMode));
		    }
#endif
            return RemapVariantName(assetBundleName)
                .Then(bundleName => LoadAssetBundleAsync(bundleName))
                .Then(bundle => AsyncManager.AddOperation(SceneManager.LoadSceneAsync(levelName, loadMode)));
		}
	} 
}
