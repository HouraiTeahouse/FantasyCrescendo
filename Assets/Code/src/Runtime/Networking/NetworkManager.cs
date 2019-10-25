using HouraiTeahouse.Attributes;
using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.Networking;
using HouraiTeahouse.Backroll;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

/// <summary>
/// Singleton MonoBehaviour manager of network related operations.
/// </summary>
public class NetworkManager : MonoBehaviour {

  /// <summary>
  /// Gets the singleton instance of NetworkManager.
  /// </summary>
	public static NetworkManager Instance { get; private set; }

  /// <summary>
  /// Gets the currently active Lobby. Null if not active.
  /// </summary>
  public Lobby Lobby { get; private set; }

  /// <summary>
  /// Gets whether both server and client are active on the current game. True if 
  /// both client and server are active.
  /// </summary>
	public bool IsHost => Lobby != null && Lobby.OwnerId == Lobby.UserId;

  /// <summary>
  /// Gets whether any network activity is currently being. True if either client 
  /// or server are active.
  /// </summary>
  public bool IsNetworkActive => Lobby != null;

	[SerializeField] [Type(typeof(IIntegrationClient), CommonName = "Integration")]
	string defaultNetworkInterface;

	[SerializeField] [Type(typeof(INetworkStrategy), CommonName = "Strategy")]
	string networkStrategy;

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()  {
		Instance = this;
	}

  /// <summary>
	/// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	/// </summary>
	// void FixedUpdate() => Host.Update();

	/// <summary>
	/// This function is called when the MonoBehaviour will be destroyed.
	/// </summary>
	void OnDestroy() { 
    if (Lobby != null) {
      LeaveLobby();
    }
  }

	Type DefaultNetworkInterfaceType => Type.GetType(defaultNetworkInterface);

	INetworkStrategy GetNetworkStrategy() {
		return Activator.CreateInstance(Type.GetType(networkStrategy)) as INetworkStrategy;
	}

	public IMatchController CreateMatchController(MatchConfig config) {
		var strategy = GetNetworkStrategy();

    // TODO(james7132): Properly set this up again
		// if (IsHost) {
		// 	return strategy.CreateHost(Host, config);
		// } else if (IsClient) {
		// 	return strategy.CreateClient(Client, config);
		// } else if (IsServer) {
		// 	return strategy.CreateServer(Server, config);
		// } else {
			return new MatchController(config);
		// }
	}

  public async Task<NetworkGameSetup> CreateLobby(IIntegrationClient integration,
                                                  MatchConfig config,
                                                  uint capacity = BackrollConstants.kMaxPlayers) {
    var lobby = await integration.LobbyManager.CreateLobby(new LobbyCreateParams {
      Type = LobbyType.Public,
      Capacity = (uint)Mathf.Min(capacity, BackrollConstants.kMaxPlayers)
    });
    if (Lobby != null) {
      Lobby.Leave();
    }
    Lobby = lobby;
    return NetworkGameSetup.InitializeLobby(lobby, config);
  }

  public async Task<NetworkGameSetup> JoinLobby(Lobby lobby) {
    await lobby.Join();
    if (Lobby != null) {
      LeaveLobby();
    }
    Lobby = lobby;
    return new NetworkGameSetup(lobby);
  }

  public void LeaveLobby() {
    if (Lobby == null) {
      Debug.LogWarning("Attempted to leave a lobby while not in a lobby.");
      return;
    }
    Lobby.Leave();
    Lobby = null;
  }

}

}