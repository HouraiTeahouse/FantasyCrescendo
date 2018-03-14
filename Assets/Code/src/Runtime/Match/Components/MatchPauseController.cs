using HouraiTeahouse.FantasyCrescendo.Matches;
using InControl;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class MatchPauseController : MonoBehaviour {

  public MatchManager MatchManager;
  public KeyCode PlayerOneKey = KeyCode.Return;
  public InputControlType PauseButton = InputControlType.Start;

  uint PausedPlayer;

  /// <summary>
  /// Update is called every frame, if the MonoBehaviour is enabled.
  /// </summary>
  void Update() {
    if (MatchManager == null || !MatchManager.IsLocal) return;
    if (MatchManager.IsPaused) {
      PausedCheck();
    } else {
      UnpausedCheck();
    }
  }

  void PausedCheck() {
    var wasPressed = WasPressed(PausedPlayer);
    if (wasPressed == true) {
      MatchManager.SetPaused(false);
    }
  }

  void UnpausedCheck() {
    for (uint i = 0; i < InputManager.Devices.Count; i++) {
      if (WasPressed(i) == true) {
        MatchManager.SetPaused(true);
        PausedPlayer = i;
        return;
      }
    }
  }

  bool? WasPressed(uint player) {
    if (PausedPlayer >= InputManager.Devices.Count) return null;
    var device = InputManager.Devices[(int)PausedPlayer];
    var wasPressed = device.GetControl(PauseButton).WasPressed;
    return wasPressed || (PausedPlayer == 0 && Input.GetKeyDown(PlayerOneKey));
  }

}

}