using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T> {

    private static T _instance;

    [SerializeField]
    private bool destroyNewInstances;

    [SerializeField]
    private bool keepBetweenScenes;

    public static T Instance {
        get {
            if (_instance == null)
                Debug.LogError("Something is trying to access the " + typeof(T).ToString() +  " Singleton instance, but none exists.");
            return _instance;
        }
    }

    protected virtual void Awake() {
        if (_instance == null) {
            _instance = this as T;
        } else {
            Destroy(destroyNewInstances ? this : _instance);
        }
        if(keepBetweenScenes)
            DontDestroyOnLoad(this);
    }

}
