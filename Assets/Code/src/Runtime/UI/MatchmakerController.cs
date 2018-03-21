using HouraiTeahouse.FantasyCrescendo.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Matchmaking {

public class MatchmakerController : MonoBehaviour {

  public GameObject NetworkMenuScreen;
  public GameObject ConnectingScreen;
  public GameObject SuccessScreen;
  public GameObject ErrorScreen;

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
    var networkManager = NetworkManager.Instance;
    var hostConfig = new NetworkHostConfig();
    Debug.Log($"Started host for matchmaking.");
    networkManager.StartHost(hostConfig);
    SetActive(SuccessScreen);
  }

  void SetActive(GameObject gameObj) {
    foreach (var screen in new[] {NetworkMenuScreen, ConnectingScreen, SuccessScreen, ErrorScreen}) {
      ObjectUtil.SetActive(screen, screen == gameObj);
    }
  }

}

}