using HouraiTeahouse.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class LobbyDisplay : MonoBehaviour, IStateView<Lobby>, IStateView<MatchmakerController> {

  public Lobby lobby;

  public Text Name;
  public Text Owner;
  public Text Players;

  MatchmakerController controller;

  /// <summary>
  /// Update is called every frame, if the MonoBehaviour is enabled.
  /// </summary>
  void Update() => UpdateUI(lobby);

  void UpdateUI(Lobby lobby) {
    if (lobby == null) return;
    var name = lobby.GetName();
    if (string.IsNullOrEmpty(name)) {
      name = "-";
    }
    SetText(Name, name);
    //SetText(Owner, info.OnwerName);
    SetText(Players, $"{lobby.MemberCount}/{lobby.Capacity}");
  }

  void SetText(Text text, string displayText) {
    if (text != null) {
      text.text = displayText;
    }
  }

  public async void JoinLobby() {
    if (controller == null || lobby == null) return;
    await controller.JoinLobby(lobby);
  }

  public void UpdateView(in Lobby lobby) {
    this.lobby = lobby;
    name = lobby.Id.ToString();
            UpdateUI(this.lobby);
  }

  public void UpdateView(in MatchmakerController controller) {
    this.controller = controller;
  }

  public void Dispose() => ObjectUtil.Destroy(this);

}

}