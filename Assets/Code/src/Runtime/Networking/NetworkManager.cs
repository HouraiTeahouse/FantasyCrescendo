using HouraiTeahouse.EditorAttributes;
using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.Matchmaking;
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
  /// Gets the currently active NetworkHost. Null if not active.
  /// </summary>
	public NetworkHost Host { get; private set; }

  /// <summary>
  /// Gets the currently active NetworkClient. Null if not active.
  /// </summary>
	public INetworkClient Client {
		get { return Host.Client; }
		private set { Host.Client = value; } 
	}

  /// <summary>
  /// Gets the currently active NetworkServer. Null if not active.
  /// </summary>
	public INetworkServer Server {
		get { return Host.Server; }
		private set { Host.Server = value; } 
	}

  /// <summary>
  /// Gets whether both server and client are active on the current game. True if 
  /// both client and server are active.
  /// </summary>
	public bool IsHost => IsClient && IsServer;

  /// <summary>
  /// Gets whether any network activity is currently being. True if either client 
  /// or server are active.
  /// </summary>
  public bool IsNetworkActive => IsClient || IsServer;

  /// <summary>
  /// Gets whether the current instance is a client to a server. True if the 
  /// NetworkClient is currently active.
  /// </summary>
	public bool IsClient => Client != null;

  /// <summary>
  /// Gets whether the current instance is server to clients. True if the 
  /// NetworkClient is currently active.
  /// </summary>
	public bool IsServer => Server != null;

  IMatchmaker matchmakerInstance;
  public IMatchmaker Matchmaker {
    get { return matchmakerInstance ?? (matchmakerInstance = (IMatchmaker)Activator.CreateInstance(Type.GetType(matchmaker))); }
  }

	[SerializeField] [Type(typeof(INetworkInterface), CommonName = "NetworkInterface")]
	string defaultNetworkInterface;

	[SerializeField] [Type(typeof(INetworkStrategy), CommonName = "Strategy")]
	string networkStrategy;

	[SerializeField] [Type(typeof(IMatchmaker), CommonName = "MatchMaker")]
	string matchmaker;

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()  {
		Instance = this;
		Host = new NetworkHost();
	}

  /// <summary>
	/// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	/// </summary>
	void FixedUpdate() => Host.Update();

	/// <summary>
	/// This function is called when the MonoBehaviour will be destroyed.
	/// </summary>
	void OnDestroy() => StopHost();

	Type DefaultNetworkInterfaceType => Type.GetType(defaultNetworkInterface);

	INetworkStrategy GetNetworkStrategy() {
		return Activator.CreateInstance(Type.GetType(networkStrategy)) as INetworkStrategy;
	}

	public IMatchController CreateMatchController(MatchConfig config) {
		var strategy = GetNetworkStrategy();
		if (IsHost) {
			return strategy.CreateHost(Host, config);
		} else if (IsClient) {
			return strategy.CreateClient(Client, config);
		} else if (IsServer) {
			return strategy.CreateServer(Server, config);
		} else {
			return new MatchController(config);
		}
	}

	// Client Methods

  /// <summary>
  /// Starts the local NetworkClient.
  /// </summary>
  /// <remarks>
  /// This does not connect the client to any server. This only opens the
  /// socket for a client.
  /// 
  /// If a client is already active, another client will not be started.
  /// and the active client will be returned.
  /// </remarks>
  /// <param name="config">the NetworkClientConfig used to start the server.</param>
  /// <param name="interfaceType">INetworkInterface type to use, uses default if null.</param>
  /// <returns>the NetworkClient created or fetched.</returns>
	public INetworkClient StartClient(NetworkClientConfig config, Type interfaceType = null) {
    if (Client != null) return Client;
    interfaceType = interfaceType ?? DefaultNetworkInterfaceType;
    Client = new NetworkGameClient(interfaceType, config);
		return Client;
	}

  /// <summary>
  /// Stops the local NetworkClient.
  /// </summary>
  /// <remarks>
  /// This will forcibly disconnect the client from any connected servers.
  /// </remarks>
	public void StopClient() {
		if (!IsClient) return;
		Client.Dispose();
		Client = null;
    Debug.Log("Client Stopped");
	}

	// Server Methods

  /// <summary>
  /// Starts the local NetworkServer.
  /// </summary>
  /// <remarks>
  /// This does not connect the server to any client. This only opens the
  /// socket for a server.
  /// 
  /// If a server is already active, another server will not be started.
  /// and the active server will be returned.
  /// </remarks>
  /// <param name="config">the NetworkServerConfig used to start the server.</param>
  /// <param name="interfaceType">INetworkInterface type to use, uses default if null.</param>
  /// <returns>the NetworkServer created or fetched.</returns>
	public async Task<INetworkServer> StartServer(NetworkServerConfig config, Type interfaceType = null) {
    if (Server != null) return Server;
    interfaceType = interfaceType ?? DefaultNetworkInterfaceType;
		Server = new NetworkGameServer(interfaceType, config);
    await Server.Initialize();
    return Server;
	}

  /// <summary>
  /// Stops the local NetworkServer.
  /// </summary>
  /// <remarks>
  /// This will forcibly disconnect all connected clients.
  /// </remarks>
	public void StopServer() {
		if (!IsServer) return;
		Server.Dispose();
		Server = null;
    Debug.Log("Server Stopped.");
	}

	// Host Methods

  /// <summary>
  /// Starts the local NetworkServer and NetworkClient, and connects the client to the
  /// local server.
  /// </summary>
  /// <remarks>
  /// This will open a connection and does not immediately resolve. To assure the
  /// connection is complete. Await the returned task.
  /// 
  /// If a client is already active, another client will not be started.
  /// and the active client will be returned.
  /// </remarks>
  /// <param name="config">the NetworkHostConfig used to start the host.</param>
  /// <param name="interfaceType">INetworkInterface type to use, uses default if null.</param>
  /// <returns>an awaitable task for the resultant NetworkHost</returns>
	public async Task<NetworkHost> StartHost(NetworkHostConfig config, Type interfaceType = null) {
		var server = await StartServer(config.ServerConfig, interfaceType) as NetworkGameServer;
    if (Client != null) StopClient();
    Client = server.CreateLocalClient();
		return Host;
	}

  /// <summary>
  /// Stops both the local client and server..
  /// </summary>
  /// <remarks>
  /// This will forcibly disconnect all connected peers.
  /// </remarks>
	public void StopHost() {
		StopClient();
		StopServer();
	}

}

}