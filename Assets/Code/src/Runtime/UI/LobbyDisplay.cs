using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo.Matchmaking {

public class LobbyDisplay : MonoBehaviour, IStateView<LobbyInfo> {

  public LobbyInfo lobbyInfo;

  public Text Name;
  public Text Owner;
  public Text Players;

  /// <summary>
  /// Update is called every frame, if the MonoBehaviour is enabled.
  /// </summary>
  void Update() => UpdateUI(lobbyInfo);

  void UpdateUI(LobbyInfo info) {
    if (info == null) return;
    var name = string.IsNullOrEmpty(info.Name) ? "-" : info.Name;
    SetText(Name, name);
    SetText(Owner, info.OnwerName);
    SetText(Players, $"{info.CurrentPlayers}/{info.MaxPlayers}");
  }

  void SetText(Text text, string displayText) {
    if (text != null) {
      text.text = displayText;
    }
  }

  public void ApplyState(LobbyInfo lobby) {
    lobbyInfo = lobby;
    name = lobby.Id.ToString();
    UpdateUI(lobbyInfo);
  }

}

}