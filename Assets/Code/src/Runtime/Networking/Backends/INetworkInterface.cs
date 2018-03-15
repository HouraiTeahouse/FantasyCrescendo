using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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
  public uint Port;
}

public interface INetworkInterface : IDisposable {

  MessageHandlers MessageHandlers { get; }
  IReadOnlyCollection<NetworkConnection> Connections { get; }

  event Action<NetworkConnection> OnPeerConnected;
  event Action<NetworkConnection> OnPeerDisconnected;

  void Initialize(uint port);
  void Update();

  void Send(int connectionId, byte[] bytes, int count, NetworkReliablity reliablity);
  void Disconnect(int connectionId);

  ConnectionStats GetConnectionStats(int connectionId);

  Task<NetworkConnection> Connect(string ip, int port);
}

}
