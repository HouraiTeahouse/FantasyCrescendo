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
            T loadedObject = Resources.Load<T>(_path);
            if (loadedObject == null)
                Debug.LogError("Tried to load asset of type" + typeof (T) + " at " + _path + " and found nothing.");
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

    }

}
