using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hourai {

    [Serializable]
    public class Resource<T> where T : Object {

        [SerializeField]
        private string _path;

        public Resource(string path) {
            if (path == null)
                throw new ArgumentNullException("path");
            _path = path;
        }

        public string Path {
            get { return _path; }
        }

        public bool IsLoaded {
            get { return Asset; }
        }

        public T Asset { get; private set; }

        public T Load() {
            if (IsLoaded)
                return Asset;
            T loadedObject = Resources.Load<T>(_path);
            if (loadedObject == null)
                Debug.LogError("Tried to load asset of type" + typeof (T) + " at " + _path + " and found nothing.");
            Asset = loadedObject;
            return Asset;
        }

        public virtual void Unload() {
            if (IsLoaded)
                Resources.UnloadAsset(Asset);
            Asset = null;
        }

    }

}