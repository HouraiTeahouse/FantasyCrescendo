using HouraiTeahouse.EditorAttributes;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class NetworkManager : MonoBehaviour {

	public static NetworkManager Instance { get; private set; }

	public NetworkHost Host { get; private set; }

	public INetworkClient Client {
		get { return Host.Client; }
		private set { Host.Client = value; } 
	}

	public INetworkServer Server {
		get { return Host.Server; }
		private set { Host.Server = value; } 
	}

	public bool IsHost => IsClient && IsServer;
	public bool IsClient => Client != null;
	public bool IsServer => Server != null;

	[SerializeField] [Type(typeof(INetworkInterface), CommonName = "NetworkInterface")]
	string networkInterface;

	[SerializeField] [Type(typeof(INetworkStrategy), CommonName = "Strategy")]
	string networkStrategy;

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

	Type NetworkInterfaceType => Type.GetType(networkInterface);

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
			return new GameController(config);
		}
	}

	// Client Methods

	public INetworkClient StartClient(NetworkClientConfig config) {
		return Client ?? (Client = new NetworkGameClient(NetworkInterfaceType, config));
	}

	public void StopClient() {
		if (!IsClient) return;
		Client.Dispose();
		Client = null;
    Debug.Log("Client Stopped");
	}

	// Server Methods

	public INetworkServer StartServer(NetworkServerConfig config) {
		return Server ?? (Server = new NetworkGameServer(NetworkInterfaceType, config));
	}

	public void StopServer() {
		if (!IsServer) return;
		Server.Dispose();
		Server = null;
    Debug.Log("Server Stopped.");
	}

	// Host Methods

	public async Task<NetworkHost> StartHost(NetworkHostConfig config) {
		StartServer(config.ServerConfig);
		StartClient(config.ClientConfig);
		await Client.Connect("localhost", (uint)config.ServerConfig.Port);
		return Host;
	}

	public void StopHost() {
		StopClient();
		StopServer();
	}

}

}