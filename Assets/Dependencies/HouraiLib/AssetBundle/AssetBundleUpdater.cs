using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.AssetBundles {

    public static class AssetBundleUpdater {

        static readonly ILog log = Log.GetLogger(typeof(AssetBundleUpdater));

#if UNITY_CLOUD_BUILD
        public const bool Updateable = true;
#elif UNITY_EDITOR 
        public const bool Updateable = false;
#else
        public static bool Updateable {
            get { return !Debug.isDebugBuild; }
        }
#endif

        public static ReadOnlyCollection<UnityWebRequest> ActiveRequests { get; private set; }
        static readonly List<UnityWebRequest> _requests;

        static AssetBundleUpdater() {
            _requests = new List<UnityWebRequest>();
            ActiveRequests = new ReadOnlyCollection<UnityWebRequest>(_requests);
        }

        public static ITask UpdateGame() {
            if (!Updateable) {
                log.Info("Build is a debug build. Not downloading updates...");
                return Task.Resolved;
            }
            log.Info("Starting game update checks...");
            var bundles = new HashSet<string>();
            var platformName = BundleUtility.GetPlatformName();
            var localManifestPath = BundleUtility.GetLocalBundlePath(platformName);
            var manifestUrl = BundleUtility.GetRemoteBundleUri(platformName);
            var manifestRequest = UnityWebRequest.Get(manifestUrl);
            log.Info("Downloading remote manifest from {0}...", manifestUrl);
            var task = AsyncManager.AddOperation(manifestRequest.Send()).Then(() => {
                if (manifestRequest.isError)
                    throw new Exception("HTTP Error: {0}".With(manifestRequest.error));
                log.Info("Downloaded remote manifest bundle ({0} bytes)", manifestRequest.downloadedBytes);
                Directory.CreateDirectory(Path.GetDirectoryName(localManifestPath));
                File.WriteAllBytes(localManifestPath, manifestRequest.downloadHandler.data);
                manifestRequest.Dispose();
                return AsyncManager.AddOperation(AssetBundle.LoadFromFileAsync(localManifestPath));
            }).Then(create => {
                log.Info("Loaded local manifest bundle from {0}", localManifestPath);
                var bundle = create.assetBundle;
                return AsyncManager.AddOperation(bundle.LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest"))
                        .Then(request => {
                            log.Info("Loaded local manifest.");
                            var manifest = request.asset as AssetBundleManifest;
                            bundles.UnionWith(manifest.GetAllAssetBundles());
                            log.Info("Unloading local manifest bundle.");
                            bundle.Unload(false);
                            return manifest;
                        });
            }).Then(manifest => {
                bundles.UnionWith(manifest.GetAllAssetBundles());
                var toDownload = bundles.Where(name => !File.Exists(BundleUtility.GetLocalBundlePath(name)));
                if (!toDownload.Any()) {
                    log.Info("Nothing to download. Ending updater...");
                    return Task.Resolved;
                }
                log.Info("Downloading new bundles...");
                return Task.All(toDownload.Select(name => {
                    var bundleUri = BundleUtility.GetRemoteBundleUri(name);
                    var bundleRequest = UnityWebRequest.Get(bundleUri);
                    log.Info("Downloading {0} from {1}", name, bundleUri);
                    return AsyncManager.AddOperation(bundleRequest.Send()).Then(() => {
                        if (bundleRequest.isError)
                            throw new Exception("HTTP Error: {0}".With(manifestRequest.error));
                        var path = BundleUtility.GetLocalBundlePath(name);
                        log.Info("Asset bundle {0} downloaded ({2} bytes), saving to {1}",
                            name,
                            path,
                            bundleRequest.downloadedBytes);
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                        File.WriteAllBytes(path, bundleRequest.downloadHandler.data);
                        bundleRequest.Dispose();
                    });
                }));
            }).Then(() => {
                log.Info("Deleting unused bundles...");
                // Bundles acts as a whitelist for valid files
                bundles.Add(platformName);
                var localStorage = BundleUtility.GetLocalStoragePath() + "/";
                foreach (string bundle in Directory.GetFiles(localStorage, "*", SearchOption.AllDirectories)) {
                    var bundleName = bundle.Replace(localStorage, "").Replace("\\", "/");
#if UNITY_EDITOR
                    if (bundle.Contains(".meta"))
                        continue;
#endif
                    if (bundles.Contains(bundleName))
                        continue;
                    log.Info("Deleting {0} ({1})", bundleName, bundle);
                    File.Delete(bundle);
                }
                // Delete empty subdirectories
                bool deleted = false;
                do {
                    deleted = false;
                    foreach (var directory in Directory.GetDirectories(localStorage, "*", SearchOption.AllDirectories)) {
                        if (Directory.GetFiles(directory).Any() || Directory.GetDirectories(directory).Any())
                            continue;
                        deleted = true;
                        Directory.Delete(directory);
                        log.Info("Deleted empty directory {0}.", directory);
                    }
                }
                while (deleted);
            });
            task.Catch(ex => { log.Error(ex.ToString()); });
            return task;
        }

    }

}

