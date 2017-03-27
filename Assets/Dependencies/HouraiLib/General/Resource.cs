using System;
using System.Collections.Generic;
using HouraiTeahouse.AssetBundles;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using HouraiTeahouse.Editor;
#endif

namespace HouraiTeahouse {

    public class Resource {

        protected static readonly ILog log = Log.GetLogger<Resource>();
        public const char BundleSeperator = ':';

        protected static readonly Dictionary<string, Resource> _resources;
        static Resource() {
            _resources = new Dictionary<string, Resource>();
        }

        public static Resource<T> Get<T>(string location) where T : Object { return Resource<T>.Get(location); }

    }

    /// <summary> A simple object that encapsulates the operations on a dynamically loaded asset using UnityEngine.Resources. </summary>
    /// <typeparam name="T"> the type of the asset encapsulated by the Resouce </typeparam>
    [Serializable]
    public sealed class Resource<T> : Resource where T : Object {

        [SerializeField]
        readonly string _path;

        public static Resource<T> Get(string location) {
            if (!_resources.ContainsKey(location))
                _resources[location] = new Resource<T>(location);
            return _resources[location] as Resource<T>;
        }

        /// <summary> Initializes a new instance of Resource with a specified Resources file path. </summary>
        /// <param name="path"> the Resourrces file path to the asset </param>
        Resource(string path) {
            _path = path ?? string.Empty;
        }

        /// <summary> The Resources path that the asset is stored at. </summary>
        public string Path {
            get { return _path; }
        }

        /// <summary> Checks whether the </summary>
        public bool IsBundled {
            get { return _path.IndexOf(BundleSeperator) >= 0; }
        }

        /// <summary> Whether the asset has been loaded in or not. </summary>
        public bool IsLoaded {
            get { return Asset; }
        }

        /// <summary> The asset handled by the Resource. Will be null if it has not been loaded yet. </summary>
        public T Asset { get; private set; }

        ITask<T> LoadTask { get; set; }

        /// <summary> Loads the asset specifed by the Resource into memory. </summary>
        /// <returns> the loaded asset </returns>
        public T Load() {
            if (IsLoaded)
                return Asset;
            T loadedObject;
            if (IsBundled)
#if UNITY_EDITOR
                loadedObject = Assets.LoadBundledAsset(_path) as T;
#else
                throw new InvalidOperationException("Cannot synchronously load assets from AssetBundles. Path: {0}".With(_path));
#endif
            else
                loadedObject = Resources.Load<T>(_path);
#if UNITY_EDITOR
            if (EditorApplication.isPlayingOrWillChangePlaymode)
#endif
                log.Info("Loaded {0} from {1}", typeof(T).Name, _path);
            Asset = loadedObject;
            return Asset;
        }

        /// <summary> Unloads the asset from memory. Asset will be null after this. </summary>
        public void Unload() {
            Asset = null;
            //if (LoadTask != null && LoadTask.State == TaskState.Pending)
            //    LoadTask.Reject(new Exception("Loading of {0} canceled.".With(_path)));
            LoadTask = null;
            // Logs error if trying to unload a GameObject as a whole
            if (!IsLoaded)
                return;
            if (Asset is GameObject)
                Object.Destroy(Asset);
            else
                Resources.UnloadAsset(Asset);
#if UNITY_EDITOR
            if (EditorApplication.isPlayingOrWillChangePlaymode)
#endif
                log.Info("[Resource] Unloaded \"{0}\" ({1})", _path, typeof(T).Name);
        }

        /// <summary> Loads the asset in an asynchronous manner. If no AsyncManager is currently availble, </summary>
        /// <param name="priority"> optional parameter, the priority of the resource request </param>
        /// <returns> the ResourceRequest associated with the load. Null if </returns>
        public ITask<T> LoadAsync(int priority = 0) {
            if (LoadTask != null)
                return LoadTask;
            // If no AsyncManager is available, load the assets synchrounously.
            if (AsyncManager.Instance == null || IsLoaded)
                return Task.FromResult(Asset);
            LoadTask = IsBundled ? LoadFromBundle(): LoadFromResources(priority);
            string typeName = typeof(T).Name;
#if UNITY_EDITOR
            if (EditorApplication.isPlayingOrWillChangePlaymode)
#endif
                log.Info("Started loading \"{1}\" ({0})", typeName, _path);
            LoadTask.Then(asset => {
#if UNITY_EDITOR
                if (EditorApplication.isPlayingOrWillChangePlaymode)
#endif
                    log.Info("Loaded \"{1} ({0})", typeName, _path);
                Asset = asset;
            });
            return LoadTask;
        }

        ITask<T> LoadFromBundle() {
            string[] splits = _path.Split(':');
            var operation = AssetBundleManager.LoadAssetAsync<T>(splits[0], splits[1]);
            var task = new Task();
            AsyncManager.Instance.AddOperation(operation, task);
            return task.Then(() => operation.Asset);
        }

        ITask<T> LoadFromResources(int priority) {
            ResourceRequest request = Resources.LoadAsync<T>(_path);
            request.priority = priority;
            return request.ToTask<T>();
        }

    }

}
