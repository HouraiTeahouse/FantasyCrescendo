using HouraiTeahouse.FantasyCrescendo.Matches;
using InControl;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class MatchPauseController : MonoBehaviour {

  public MatchManager MatchManager => MatchManager.Instance;
  public KeyCode PlayerOneKey = KeyCode.Return;
  public InputControlType PauseButton = InputControlType.Start;

  public int PausedPlayer;

  // TODO(james7132): Add support for non-keyboard reset.
  public KeyCode[] ResetKeys = new KeyCode[] {
    KeyCode.LeftShift,
    KeyCode.Return,
    KeyCode.Q,
    KeyCode.E,
  };

  /// <summary>
  /// Update is called every frame, if the MonoBehaviour is enabled.
  /// </summary>
  void Update() {
    if (MatchManager == null || !MatchManager.IsLocal) return;
    if (MatchManager.CurrentProgressionID == MatchProgressionState.Pause) {
      PausedCheck();
    } else if (MatchManager.CurrentProgressionID == MatchProgressionState.InGame) {
      UnpausedCheck();
    }
  }

  void PausedCheck() {
    if (MatchResetCheck()) {
      ResetMatch();
    }
    var wasPressed = WasPressed(PausedPlayer);
    if (wasPressed == true) {
      MatchManager.SetPaused(false, PausedPlayer);
    }
  }

  void UnpausedCheck() {
    if (InputManager.Devices.Count <= 0) {
      PlayerUnpausedCheck(0);
    } else {
      for (var i = 0; i < InputManager.Devices.Count; i++) {
        PlayerUnpausedCheck(i);
      }
    }
  }

  void PlayerUnpausedCheck(int player) {
    if (WasPressed(player) == true) {
      MatchManager.SetPaused(true, player);
      PausedPlayer = player;
    }
  }

  void ResetMatch() {
    // TODO(james7132): Generalize match end logic.
    MatchManager.EndMatch(new MatchResult {
      Resolution = MatchResolution.NoContest,
      WinningPlayerID = -1,
      PlayerStats = MatchResultUtil.CreateMatchStatsFromConfig(MatchManager.Config)
    });
  }

  bool MatchResetCheck() {
    foreach (KeyCode key in ResetKeys) {
      if (!Input.GetKey(key)) return false;
    }
    return true;
  }

  bool? WasPressed(int player) {
    if (player >= InputManager.Devices.Count) {
      if (player == 0) {
        return Input.GetKeyDown(PlayerOneKey);
      } else {
        return null;
      }
    }
    return InputManager.Devices[player].GetControl(PauseButton).WasPressed ||
           (player == 0 && Input.GetKeyDown(PlayerOneKey));
  }

}

}