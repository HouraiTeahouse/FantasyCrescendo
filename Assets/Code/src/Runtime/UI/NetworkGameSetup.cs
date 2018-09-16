using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public sealed class NetworkGameSetup : MonoBehaviour, IValidator<MatchConfig> {

  public GameSetupMenu GameSetupMenu;
  public CharacterSelectMenu CharacterSelectMenu;

  /// <summary>
  /// This function is called when the object becomes enabled and active.
  /// </summary>
  void OnEnable() {
    var networkManager = NetworkManager.Instance;
    if (networkManager == null) return;
    if (networkManager.IsServer) { EnableServer(networkManager.Server); }
    if (networkManager.IsClient) { EnableClient(networkManager.Client); }
  }

  /// <summary>
  /// This function is called when the behaviour becomes disabled or inactive.
  /// </summary>
  void OnDisable() {
    var networkManager = NetworkManager.Instance;
    if (networkManager == null) return;
    if (networkManager.IsClient) { DisableClient(networkManager.Client); }
    if (networkManager.IsServer) { DisableServer(networkManager.Server); }
  }

  public void CloseConnections() {
    var networkManager = NetworkManager.Instance;
    if (networkManager == null) return;
    if (networkManager.IsClient) { networkManager.StopClient(); }
    if (networkManager.IsServer) { networkManager.StopServer(); }
  }

  bool IValidator<MatchConfig>.IsValid(MatchConfig obj) {
    var networkManager = NetworkManager.Instance;
    if (networkManager == null || !networkManager.IsNetworkActive) return true;
    if (networkManager.IsServer) {
      var baseConfig = ServerBuildBaseConfig();
      return GameSetupMenu.MainMenu.CurrentGameMode.IsValidConfig(baseConfig);
    }
    // Non-host clients should not be able to start a match
    return false;
  }

  // Server Methods

  void EnableServer(INetworkServer server) {
    server.PlayerAdded += OnServerAddPlayer;
    server.PlayerUpdated += OnServerUpdatePlayer;
    server.PlayerRemoved += OnServerRemovePlayer;

    GameSetupMenu.OnStartMatch += OnServerStartMatch;

    foreach (var client in server.Clients)  {
      InitalizePlayer(client);
    }
    OnServerUpdatedConfig();
  }

  void DisableServer(INetworkServer server) {
    server.PlayerAdded -= OnServerAddPlayer;
    server.PlayerUpdated -= OnServerUpdatePlayer;
    server.PlayerRemoved -= OnServerRemovePlayer;
    GameSetupMenu.OnStartMatch -= OnServerStartMatch;
  }

  Task OnServerStartMatch(MatchConfig config) {
    var baseConfig = ServerBuildBaseConfig();
    foreach (var player in NetworkManager.Instance.Server.Clients) {
      player.StartMatch(BuildConfigForPlayer(baseConfig, player.PlayerID));
    }
    return Task.CompletedTask;
  }

  void OnServerAddPlayer(NetworkClientPlayer player) {
    InitalizePlayer(player);
    OnServerUpdatedConfig(); 
  }
  void OnServerUpdatePlayer(NetworkClientPlayer player) {
    Debug.Log($"Player config updated: {player.PlayerID} {player.Config}");
    OnServerUpdatedConfig();
  } 
  void OnServerRemovePlayer(int playerId) => OnServerUpdatedConfig();

  void InitalizePlayer(NetworkClientPlayer player) {
    player.Config.Selection = CharacterSelectMenu.CreateNewSelection(player.PlayerID);
  }

  void OnServerUpdatedConfig() {
    var baseConfig = ServerBuildBaseConfig();
    Debug.Log($"Sending Config: {baseConfig}");
    foreach (var player in NetworkManager.Instance.Server.Clients) {
      player.SendConfig(BuildConfigForPlayer(baseConfig, player.PlayerID));
    }
  }

  MatchConfig ServerBuildBaseConfig() {
    var baseConfig = GameSetupMenu.Config;
    var server = NetworkManager.Instance.Server;
    baseConfig.PlayerConfigs = (from client in server.Clients orderby client.PlayerID select client.Config).ToArray();
    return baseConfig;
  }

  MatchConfig BuildConfigForPlayer(MatchConfig baseConfig, uint playerId) {
    // TODO(james7132): Generalize this to work with multiple players per client
    for (var i = 0; i < baseConfig.PlayerConfigs.Length; i++) {
      baseConfig.PlayerConfigs[i].LocalPlayerID = -1;
    }
    baseConfig.PlayerConfigs[playerId].LocalPlayerID = 0;
    return baseConfig;
  }

  // Client Methods

  void EnableClient(INetworkClient client) {
    client.OnMatchConfigUpdated += OnClientUpdatedConfig;
    client.OnMatchStarted += OnClientMatchStarted;
    foreach (var player in CharacterSelectMenu.Players) {
      player.PlayerUpdated += OnClientLocalPlayerUpdated;
    }
  }

  void DisableClient(INetworkClient client) {
    client.OnMatchConfigUpdated -= OnClientUpdatedConfig;
    client.OnMatchStarted -= OnClientMatchStarted;
    foreach (var player in CharacterSelectMenu.Players) {
      player.PlayerUpdated -= OnClientLocalPlayerUpdated;
    }
  }

  void OnClientLocalPlayerUpdated(byte playerId, PlayerConfig config) {
    var client = NetworkManager.Instance.ForceNull()?.Client;
    if (client == null) return;
    client.SetConfig(config);
  }

  void OnClientUpdatedConfig(MatchConfig config)  {
    CharacterSelectMenu.ApplyState(config); 
  }

  async void OnClientMatchStarted(MatchConfig config) {
    await GameSetupMenu.MainMenu.CurrentGameMode.Execute(config);
  }

}

}
