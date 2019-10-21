using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Steamworks;
using HouraiTeahouse.Networking;
using HouraiTeahouse.FantasyCrescendo.Matchmaking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class NetworkHost : IDisposable {

  public INetworkServer Server { get; set; }
  public INetworkClient Client { get; set; }

	public static NetworkHost Create(Type interfaceType, NetworkHostConfig config) {
		return new NetworkHost {
			Server = new NetworkGameServer(interfaceType, config.ServerConfig),
			Client = new NetworkGameClient(interfaceType, config.ClientConfig),
		};
	}

  public void Update() {
    Server?.Update();
    Client?.Update();
  }

  public void Dispose() {
    Server?.Dispose();
    Client?.Dispose();
  }

}

public struct NetworkHostConfig {
  public NetworkClientConfig ClientConfig;
  public NetworkServerConfig ServerConfig;
}

public struct NetworkClientConfig {
}

public struct NetworkServerConfig {
  public int Port;
}

public enum NetworkInterfaceType {
  Client,
  Server
}

public class NetworkInterfaceConfiguration {
  public NetworkInterfaceType Type;
  public int Port;

  public ELobbyType ServerSteamLobbyType = ELobbyType.k_ELobbyTypePublic;
  public int ServerSteamLobbyMaxSize = (int)GameMode.GlobalMaxPlayers;
}

public class NetworkConnectionConfig {
  public string IP;
  public int Port;

  public LobbyInfo LobbyInfo;
}

public interface INetworkInterface : IDisposable {

  MessageHandlers MessageHandlers { get; }
  IReadOnlyCollection<NetworkConnection> Connections { get; }

  event Action<NetworkConnection> OnPeerConnected;
  event Action<NetworkConnection> OnPeerDisconnected;

  Task Initialize(NetworkInterfaceConfiguration config);
  void Update();

  void Send(int connectionId, byte[] bytes, int count, NetworkReliablity reliablity);

  Task<NetworkConnection> Connect(NetworkConnectionConfig config);
  void Disconnect(int connectionId);

  ConnectionStats GetConnectionStats(int connectionId);

}

}
