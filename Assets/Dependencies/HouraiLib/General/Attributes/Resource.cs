using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hourai {

    [Serializable]
    public class Resource<T> where T : Object {

        private readonly string _path;

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
            Object loadedObject = Resources.Load(_path);
            if (loadedObject != null) {
                var asT = loadedObject as T;
                if (asT != null)
                    Asset = asT;
                else {
                    Debug.LogError("Tried to load asset of type" + typeof (T) + " at " + _path +
                                   " and found an Object of type " + loadedObject.GetType() + " instead");
                }
                return Asset;
            }
            Debug.LogError("Tried to load asset of type" + typeof (T) + " at " + _path + " and found nothing.");
            return null;
        }

        public virtual void Unload() {
            if (IsLoaded)
                Resources.UnloadAsset(Asset);
            Asset = null;
        }

    }

}