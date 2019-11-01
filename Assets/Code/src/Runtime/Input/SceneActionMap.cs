using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class SceneActionMap : MonoBehaviour {

    [SerializeField] bool _setOnAwake = true;
    [SerializeField] string _sceneDefaultActionMap;

    void Awake() {
        if (_setOnAwake) {
            SetActionMap(_sceneDefaultActionMap);
        }
    }

    public void SetActionMap(string actionMap) {
        var manager = InputManager.Instance;
        if (manager == null) {
            Debug.LogWarning("Unable to set the scene's InputActionMap: InputManager missing.");
            return;
        }
        manager.ChangeActiveActionMap(actionMap);
    }

}

}
