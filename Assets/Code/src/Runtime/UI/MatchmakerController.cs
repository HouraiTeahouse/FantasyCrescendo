using HouraiTeahouse.FantasyCrescendo.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo.Matchmaking {

public class MatchmakerController : MonoBehaviour {

  public GameObject NetworkMenuScreen;
  public GameObject ConnectingScreen;
  public GameObject SuccessScreen;
  public GameObject ErrorScreen;

  public Text ErrorText;

  public float RefreshTimer = 1f;
  float timer;

  void OnEnable() => Refresh();

  public async void Refresh() => await RefreshLobbies();

  /// <summary>
  /// Update is called every frame, if the MonoBehaviour is enabled.
  /// </summary>
  void Update() {
    if (Time.unscaledTime- timer > RefreshTimer) {
      Refresh();
      timer = Time.unscaledTime;
    }
  }

  async Task RefreshLobbies() {
    var networkManager = NetworkManager.Instance;
    if (networkManager == null) return;
    var lobbies = await networkManager.Matchmaker.GetLobbies();
    foreach (var display in GetComponentsInChildren<IStateView<IEnumerable<LobbyInfo>>>()) {
      display.ApplyState(lobbies);
    }
    foreach (var display in GetComponentsInChildren<IStateView<MatchmakerController>>()) {
      display.ApplyState(this);
    }
  }

  public async void CreateLobby() {
    Debug.Log("Started host for matchmaking.");
    SetActive(ConnectingScreen);
    try {
      var manager = NetworkManager.Instance;
      await manager.StartHost(new NetworkHostConfig(), manager.Matchmaker.NetworkInterfaceType);
      SetActive(SuccessScreen);
    } catch (NetworkingException exception) {
      SetActive(ErrorScreen);
      ErrorText.text = exception.Message;
    }
  }

  public async Task JoinLobby(LobbyInfo lobby) {
    if (lobby == null) return;
    var manager = NetworkManager.Instance;
    var client = manager.StartClient(new NetworkClientConfig(), manager.Matchmaker.NetworkInterfaceType);
    try {
      SetActive(ConnectingScreen);
      await client.Connect(new NetworkConnectionConfig {
        LobbyInfo = lobby
      });
      SetActive(SuccessScreen);
    } catch (NetworkingException exception)  {
      SetActive(ErrorScreen);
      ErrorText.text = exception.Message;
    }
  }

  void SetActive(GameObject gameObj) {
    foreach (var screen in new[] {NetworkMenuScreen, ConnectingScreen, SuccessScreen, ErrorScreen}) {
      ObjectUtil.SetActive(screen, screen == gameObj);
    }
  }

}

}