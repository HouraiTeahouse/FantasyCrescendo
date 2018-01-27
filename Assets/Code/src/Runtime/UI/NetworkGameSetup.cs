using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public sealed class NetworkGameSetup : MonoBehaviour {

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
    if (networkManager.IsClient) { 
      DisableClient(networkManager.Client); 
      networkManager.StopClient();
    }
    if (networkManager.IsServer) { 
      DisableServer(networkManager.Server); 
      networkManager.StopServer();
    }
  }

  // Server Methods

  void EnableServer(INetworkServer server) {
    server.PlayerAdded += OnServerAddPlayer;
    server.PlayerUpdated += OnServerUpdatePlayer;
    server.PlayerRemoved += OnServerRemovePlayer;
  }

  void DisableServer(INetworkServer server) {
    server.PlayerAdded -= OnServerAddPlayer;
    server.PlayerUpdated -= OnServerUpdatePlayer;
    server.PlayerRemoved -= OnServerRemovePlayer;
  }

  void OnServerAddPlayer(NetworkClientPlayer player) => OnServerUpdatedConfig();
  void OnServerUpdatePlayer(NetworkClientPlayer player) => OnServerUpdatedConfig();
  void OnServerRemovePlayer(uint playerId) => OnServerUpdatedConfig();

  void OnServerUpdatedConfig() {
    var baseConfig = CharacterSelectMenu.Config;
    var server = NetworkManager.Instance.Server;
    baseConfig.PlayerConfigs = (from client in server.Clients orderby client.PlayerID select client.Config).ToArray();
    foreach (var player in server.Clients) {
      player.SendConfig(BuildPlayerMatchConfig(baseConfig, player.PlayerID));
    }
  }

  MatchConfig BuildPlayerMatchConfig(MatchConfig baseConfig, uint playerId) {
    // TODO(james7132): Generalize this to work with multiple players per client
    for (var i = 0; i < baseConfig.PlayerConfigs.Length; i++) {
      baseConfig.PlayerConfigs[i].LocalPlayerID = -1;
    }
    baseConfig.PlayerConfigs[playerId].LocalPlayerID = 1;
    return baseConfig;
  }

  // Client Methods

  void EnableClient(INetworkClient client) {
    client.OnMatchConfigUpdated += OnClientUpdatedConfig;
  }

  void DisableClient(INetworkClient client) {
    client.OnMatchConfigUpdated -= OnClientUpdatedConfig;
  }

  void OnClientUpdatedConfig(MatchConfig config) => CharacterSelectMenu.ApplyState(config);

}

}
