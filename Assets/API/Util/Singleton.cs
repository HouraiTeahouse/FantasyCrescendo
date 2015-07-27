using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T> {

    private static T _instance;

	[SerializeField]
	private bool _dontDestroyOnLoad;

    public static T Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<T>();
                if (_instance == null) {
                    Debug.LogError("Something is trying to access the " + typeof(T) + " Singleton instance, but none exists.");      
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake() {
        if (_instance == null) {
			_instance = this as T;
			if(_dontDestroyOnLoad)
				DontDestroyOnLoad(this);
		} else {
			Destroy(gameObject);
		}
    }

}
