using HouraiTeahouse.FantasyCrescendo.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Matchmaking {

public class MatchmakerController : MonoBehaviour {

  async void OnEnable() {
    var networkManager = NetworkManager.Instance;
    if (networkManager == null) return;
    var lobbies = await networkManager.Matchmaker.GetLobbies();
    foreach (var display in GetComponentsInChildren<IStateView<IEnumerable<LobbyInfo>>>()) {
      display.ApplyState(lobbies);
    }
  }

  public async void CreateLobby() {
    var lobby = await NetworkManager.Instance.Matchmaker.CreateLobby();
  }

}

}