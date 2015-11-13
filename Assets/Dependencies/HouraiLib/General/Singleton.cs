using UnityEngine;
namespace Hourai {

    public abstract class Singleton<T> : HouraiBehaviour where T : Singleton<T> {

        private static T _instance;

        [SerializeField]
        private bool _dontDestroyOnLoad = false;

        public static T Instance {
            get {
                if (_instance == null) {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null) {
                        Debug.LogError("Something is trying to access the " + typeof(T) +
                                       " Singleton instance, but none exists.");
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake() {
            base.Awake();
            if (_instance == null) {
                _instance = this as T;
                if (_dontDestroyOnLoad)
                    DontDestroyOnLoad(this);
            } else {
                if (_instance == this)
                    return;
                Debug.Log("Destroying " + gameObject + " because " + _instance + " already exists.");
                Destroy(gameObject);
            }
        }
    }
}