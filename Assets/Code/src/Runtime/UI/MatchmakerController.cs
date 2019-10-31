using HouraiTeahouse.Networking;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System;
using UnityEngine.UI;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

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

  IEnumerable<T> RoundRobin<T>(IList<T>[] sequences) {
    var index = 0;
    var pushed = false;
    do {
      pushed = false;
      foreach (var seq in sequences) {
        if (index >= seq.Count) continue;
        yield return seq[index];
        pushed = true;
      }
      index++;
    } while (pushed);
  }

  async Task RefreshLobbies() {
    var integrationManager = IntegrationManager.Instance;
    if (integrationManager == null || integrationManager.Integrations == null) return;
    var tasks = new List<Task<IList<Lobby>>>();
    foreach (var integration in integrationManager.Integrations) {
      tasks.Add(integration.LobbyManager.SearchLobbies());
    }
    Lobby[] lobbies = RoundRobin<Lobby>(await Task.WhenAll(tasks)).ToArray();
    foreach (var display in GetComponentsInChildren<IStateView<IEnumerable<Lobby>>>()) {
      display.UpdateView(lobbies);
    }
    foreach (var display in GetComponentsInChildren<IStateView<MatchmakerController>>()) {
      var matchMaker = this;
      display.UpdateView(matchMaker);
    }
  }

  public async void CreateLobby() {
    var integrationManager = IntegrationManager.Instance;
    if (integrationManager == null || integrationManager.Integrations?.Count <= 0) {
      throw new InvalidOperationException("Cannot create a lobby without a running ");
    }
    Debug.Log("Started host for matchmaking.");
    SetActive(ConnectingScreen);
    try {
      var manager = NetworkManager.Instance;
      // TODO(james7132): Have this prefer one integration over another.
      // TODO(james7132): Allow initial configuration through.
      var initialConfig = new MatchConfig { Stocks = 3 };
      var networkSetup = await manager.CreateLobby(integrationManager.Integrations.FirstOrDefault(),
                                                   initialConfig);
      SetActive(SuccessScreen);
    } catch (Exception exception) {
      SetActive(ErrorScreen);
      ErrorText.text = exception.Message;
    }
  }

  public async Task JoinLobby(Lobby lobby) {
    try {
      SetActive(ConnectingScreen);
      await NetworkManager.Instance.JoinLobby(lobby);
      SetActive(SuccessScreen);
    } catch (Exception exception) {
      SetActive(ErrorScreen);
      ErrorText.text = exception.Message;
    }
  }

  void SetActive(GameObject gameObj) {
    foreach (var screen in new[] {NetworkMenuScreen, ConnectingScreen, SuccessScreen, ErrorScreen}) {
      ObjectUtility.SetActive(screen, screen == gameObj);
    }
  }

}

}