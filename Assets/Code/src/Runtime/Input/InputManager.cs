using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using PlayerInputComponent = UnityEngine.InputSystem.PlayerInput;

namespace HouraiTeahouse.FantasyCrescendo {

[RequireComponent(typeof(PlayerInputManager))]
public class InputManager : MonoBehaviour {

    public static InputManager Instance { get; private set; }

    Dictionary<int, PlayerInputWatcher> _inputWatchers;
    string currentActionMap;

    void Awake() {
        Instance = this;
        DontDestroyOnLoad(this);
        _inputWatchers = new Dictionary<int, PlayerInputWatcher>();
        var playerInputPrefab = GetComponent<PlayerInputManager>().playerPrefab;
        if (playerInputPrefab == null) return;
        var inputComponent = playerInputPrefab.GetComponent<PlayerInputComponent>();
        if (inputComponent == null) return;
        currentActionMap = inputComponent.defaultActionMap;
    }

    public PlayerInput GetInputForPlayer(int playerId) {
        if (_inputWatchers.TryGetValue(playerId, out PlayerInputWatcher watcher)) {
            Assert.IsNotNull(watcher);
            return watcher.GetLatestInputs();
        }
        return default(PlayerInput);
    }

    public void ChangeActiveActionMap(string actionMap) {
        Assert.IsNotNull(actionMap);
        currentActionMap = actionMap;
        foreach (var inputComponent in PlayerInputComponent.all) {
            inputComponent.SwitchCurrentActionMap(currentActionMap);
        }
        Debug.Log($"[InputManager] Set active action map to: {currentActionMap}");
    }

    internal void Register(int playerIndex, PlayerInputWatcher watcher) {
        Assert.IsNotNull(watcher);
        _inputWatchers.Add(playerIndex, watcher);

        // Attach it as a child so that it doesn't get destroyed
        watcher.transform.parent = transform;
        watcher.transform.localPosition = Vector3.zero;
        watcher.transform.localRotation = Quaternion.identity;

        var inputComponent = watcher.GetComponent<PlayerInputComponent>();
        if (currentActionMap == null || inputComponent == null) return;
        inputComponent.SwitchCurrentActionMap(currentActionMap);
    }

}


}
