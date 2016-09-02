using System;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace HouraiTeahouse {

    /// <summary> A simple object that encapsulates the operations on a dynamically loaded asset using UnityEngine.Resources. </summary>
    /// <typeparam name="T"> the type of the asset encapsulated by the Resouce </typeparam>
    [Serializable]
    public sealed class Resource<T> where T : Object {

        [SerializeField]
        readonly string _path;

        /// <summary> Initializes a new instance of Resource with a specified Resources file path. </summary>
        /// <param name="path"> the Resourrces file path to the asset </param>
        public Resource(string path) {
            _path = path ?? string.Empty;
        }

        /// <summary> The Resources path that the asset is stored at. </summary>
        public string Path {
            get { return _path; }
        }

        /// <summary> Whether the asset has been loaded in or not. </summary>
        public bool IsLoaded {
            get { return Asset; }
        }

        /// <summary> The asset handled by the Resource. Will be null if it has not been loaded yet. </summary>
        public T Asset { get; private set; }

        /// <summary> Loads the asset specifed by the Resource into memory. </summary>
        /// <returns> the loaded asset </returns>
        public T Load(Action<T> callback = null) {
            if (!IsLoaded) {
                var loadedObject = Resources.Load<T>(_path);
#if UNITY_EDITOR
                if (EditorApplication.isPlayingOrWillChangePlaymode)
#endif
                    Log.Info("Loaded {0} from {1}", typeof(T).Name, _path);
                Asset = loadedObject;
            }
            callback.SafeInvoke(Asset);
            return Asset;
        }

        /// <summary> Unloads the asset from memory. Asset will be null after this. </summary>
        public void Unload() {
            Asset = null;
            // Logs error if trying to unload a GameObject as a whole
            if (!IsLoaded || Asset is GameObject)
                return;
            Resources.UnloadAsset(Asset);
#if UNITY_EDITOR
            if (EditorApplication.isPlayingOrWillChangePlaymode)
#endif
                Log.Info("Unloaded {0}", _path);
        }

        /// <summary> Loads the asset in an asynchronous manner. If no AsyncManager is currently availble, </summary>
        /// <param name="callback"> optional parameter, if not null, will execute with the loaded asset as the parameter </param>
        /// <param name="priority"> optional parameter, the priority of the resource request </param>
        /// <returns> the ResourceRequest associated with the load. Null if </returns>
        public ResourceRequest LoadAsync(Action<T> callback = null, int priority = 0) {
            // Check if asset is already loaded.
            AsyncManager manager = AsyncManager.Instance;
            // If no AsyncManager is available, load the assets synchrounously.
            if (manager == null || IsLoaded) {
                Load(callback);
                return null;
            }
            ResourceRequest request = Resources.LoadAsync<T>(_path);
            request.priority = priority;
            string typeName = typeof(T).Name;
#if UNITY_EDITOR
            if (EditorApplication.isPlayingOrWillChangePlaymode)
#endif
                Log.Info("Requesting load of {0} from {1}", typeName, _path);
            manager.AddOpreation(request,
                delegate(T obj) {
#if UNITY_EDITOR
                    if (EditorApplication.isPlayingOrWillChangePlaymode)
#endif
                        Log.Info("Loaded {0} from {1}", typeName, _path);
                    Asset = obj;
                    callback.SafeInvoke(obj);
                });
            return request;
        }

    }

}