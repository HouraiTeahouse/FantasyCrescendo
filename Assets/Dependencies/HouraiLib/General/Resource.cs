using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HouraiTeahouse {
    /// <summary>
    /// A simple object that encapsulates the operations on a dynamically loaded asset using UnityEngine.Resources.
    /// </summary>
    /// <typeparam name="T">the type of the asset encapsulated by the Resouce</typeparam>
    [Serializable]
    [HelpURL("http://wiki.houraiteahouse.net/index.php/Dev:Resources#ResourcePathAttribute_and_Resource_Wrapper")]
    public sealed class Resource<T> where T : Object {
        [SerializeField]
        private readonly string _path;

        /// <summary>
        /// Initializes a new instance of Resource with a specified Resources file path.
        /// </summary>
        /// <param name="path">the Resourrces file path to the asset</param>
        public Resource(string path) {
            if (path == null)
                path = string.Empty;
            _path = path;
        }

        /// <summary>
        /// The Resources path that the asset is stored at.
        /// </summary>
        public string Path {
            get { return _path; }
        }

        /// <summary>
        /// Whether the asset has been loaded in or not.
        /// </summary>
        public bool IsLoaded {
            get { return Asset; }
        }

        /// <summary>
        /// The asset handled by the Resource. Will be null if it has not been loaded yet.
        /// </summary>
        public T Asset { get; private set; }

        /// <summary>
        /// Loads the asset specifed by the Resource into memory.
        /// </summary>
        /// <returns>the loaded asset</returns>
        public T Load() {
            if (IsLoaded)
                return Asset;
            var loadedObject = Resources.Load<T>(_path);
            Asset = loadedObject;
            return Asset;
        }

        /// <summary>
        /// Unloads the asset from memory. Asset will be null after this.
        /// </summary>
        public void Unload() {
            if (IsLoaded)
                Resources.UnloadAsset(Asset);
            Asset = null;
        }

        /// <summary>
        /// Loads the asset in an asynchronous manner. 
        /// </summary>
        /// <param name="callback">optional parameter, if not null, will execute with the loaded asset as the parameter</param>
        /// <param name="priority">optional parameter, the priority of the resource request</param>
        /// <returns>the ResourceRequest associated with the </returns>
        public ResourceRequest LoadAsync(Action<T> callback = null, int priority = 0) {
            Log.Info("Requesting load of {0} from {1}", typeof(T), _path);
            AsyncManager manager = AsyncManager.Instance;
            if(!manager)
                throw new InvalidOperationException("Cannot execute a async load without a AsyncManager instance.");
            ResourceRequest request = Resources.LoadAsync<T>(_path);
            request.priority = priority;
            manager.AddOpreation(request, delegate(T obj) {
                Log.Info("Loaded {0} from {1}", typeof(T), _path);
                Asset = obj;
                if (callback != null)
                    callback(obj);
            });
            return request;
        }
    }
}
