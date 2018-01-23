using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class NetworkHost {
  public INetworkServer Server { get; }
  public INetworkClient Client { get; }

  public NetworkHost(INetworkInterface networkInterface, NetworkHostConfig config) {
    Server = new NetworkGameServer(networkInterface, config.ServerConfig);
    Client = new NetworkGameClient(networkInterface, config.ClientConfig);
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
  IReadOnlyCollection<INetworkConnection> Connections { get; }

  Task Initialize();
  void Update();

  event Action<INetworkConnection> OnPeerConnected;
  event Action<INetworkConnection> OnPeerDisconnected;

  Task<INetworkConnection> Connect(string ip, int port);
}

}
