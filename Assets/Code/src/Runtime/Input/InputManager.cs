using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using MatchPlayerInput = HouraiTeahouse.FantasyCrescendo.PlayerInput;

namespace HouraiTeahouse.FantasyCrescendo {

[RequireComponent(typeof(PlayerInputManager))]
public class InputManager : MonoBehaviour {

  public static InputManager Instance { get; private set; }

  Dictionary<int, PlayerInputWatcher> _inputWatchers;

  void Awake() {
    Instance = this;
    _inputWatchers = new Dictionary<int, PlayerInputWatcher>();
    DontDestroyOnLoad(this);
  }

  public MatchPlayerInput GetInputForPlayer(int playerId) {
    if (_inputWatchers.TryGetValue(playerId, out PlayerInputWatcher watcher)) {
      Assert.IsNotNull(watcher);
      return watcher.GetLatestInputs();
    }
    return default(MatchPlayerInput);
  }

  internal void Register(int playerIndex, PlayerInputWatcher watcher) {
    Assert.IsNotNull(watcher);
    _inputWatchers.Add(playerIndex, watcher);
    // Attach it as a child so that it doesn't get destroyed
    watcher.transform.parent = transform;
    watcher.transform.localPosition = Vector3.zero;
    watcher.transform.localRotation = Quaternion.zero;
  }

}


}
