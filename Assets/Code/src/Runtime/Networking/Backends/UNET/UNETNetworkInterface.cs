using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking.UNET {

public class UNETNetworkInterface : INetworkInterface {

  public MessageHandlers MessageHandlers { get; }
  public IReadOnlyCollection<INetworkConnection> Connections { get; }

  readonly IList<INetworkConnection> connections;
  readonly IDictionary<int, UNETConnection> connectionMap;
  readonly IDictionary<NetworkReliablity, int> Channels;
  internal int SocketID { get; set; }

  public event Action<INetworkConnection> OnPeerConnected;
  public event Action<INetworkConnection> OnPeerDisconnected;

  public UNETNetworkInterface() {
    MessageHandlers = new MessageHandlers();
    Channels = new Dictionary<NetworkReliablity, int>();
    connectionMap = new Dictionary<int, UNETConnection>();
    connections = new List<INetworkConnection>();
    Connections = new ReadOnlyCollection<INetworkConnection>(connections);

    Debug.Log("Initalizing UNET Network Interface...");
    NetworkTransport.Init();
    Debug.Log("Initalized Transport Layer.");

    var config = new ConnectionConfig();
    AddChannel(config, QosType.Reliable, NetworkReliablity.Reliable);
    AddChannel(config, QosType.StateUpdate, NetworkReliablity.StateUpdate);
    AddChannel(config, QosType.Unreliable, NetworkReliablity.Unreliable);

    var hostTopology = new HostTopology(config, (int)GameMode.GlobalMaxPlayers);

    // TODO(james7132): Make port configurable
    SocketID = NetworkTransport.AddHost(hostTopology, 8888);

    Debug.Log("Network Interface Configured.");
    Debug.Log("UNET Network Interface Initialized.");
  }

  public Task Initialize() => Task.CompletedTask;

  public Task<INetworkConnection> Connect(string address, int port) {
    address = ResolveAddress(address);
    byte error;
    var connectionId = NetworkTransport.Connect(SocketID, address, port, 0, out error);
    UNETUtility.HandleError(error);
    // TODO(james7132): Await response
    var connection = new UNETConnection(this, connectionId);
    connectionMap.Add(connectionId, connection);
    return Task.FromResult<INetworkConnection>(connection);
  }

  internal int GetChannelID(NetworkReliablity reliability) {
    int channelId;
    if (Channels.TryGetValue(reliability, out channelId)) {
      return channelId;
    }
    return Channels[NetworkReliablity.Reliable];
  }

  public void Update() {
    const int kMaxBufferSize = 1024;
    int hostId, connectionId, channelId, dataSize, bufferSize = kMaxBufferSize;
    byte[] recBuffer = ArrayPool<byte>.Shared.Rent(kMaxBufferSize);
    byte error;
    NetworkEventType evt = NetworkTransport.Receive(out hostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
    UNETUtility.HandleError(error);
    switch (evt) {
      case NetworkEventType.Nothing: break;
      case NetworkEventType.ConnectEvent: OnNewConnection(connectionId); break;
      case NetworkEventType.DataEvent: OnRecieveData(connectionId, recBuffer, dataSize); break;
      case NetworkEventType.DisconnectEvent: OnDisconnect(connectionId); break;
      default:
        Debug.LogError($"Unkown network message type recieved: {evt}");
        break;
    }
    ArrayPool<byte>.Shared.Return(recBuffer);
  }

  void OnNewConnection(int connectionId) {
    var connection = AddConnection(connectionId);
    OnPeerConnected?.Invoke(connection);
    Debug.Log("Incoming connection event received");
  }

  void OnRecieveData(int connectionId, byte[] buffer, int dataSize) {
    UNETConnection connection;
    if (!connectionMap.TryGetValue(connectionId, out connection)) return;
    MessageHandlers.Execute(connection, buffer, dataSize);
    connection.MessageHandlers.Execute(connection, buffer, dataSize);
  }

  void OnDisconnect(int connectionId) {
    UNETConnection connection;
    if (!connectionMap.TryGetValue(connectionId, out connection)) return;
    OnPeerDisconnected?.Invoke(connection);
    RemoveConnection(connection);
    Debug.Log("Remote client event disconnected");
  }

  public void Dispose() => NetworkTransport.Shutdown();

  void AddChannel(ConnectionConfig config, QosType qos, NetworkReliablity reliablity) {
    Channels.Add(reliablity, config.AddChannel(qos));
  }

  UNETConnection AddConnection(int connectionId) {
    UNETConnection connection;
    if (!connectionMap.TryGetValue(connectionId, out connection)) {
      connection = new UNETConnection(this, connectionId);
      connectionMap[connectionId] = connection;
      connections.Add(connection);
    }
    return connection;
  }

  void RemoveConnection(UNETConnection connection) {
    connectionMap.Remove(connection.ConnectionID);
    connections.Remove(connection);
  }

  string ResolveAddress(string address) {
    if (address.Equals("127.0.0.1") || address.Equals("localhost")) {
      return "127.0.0.1";
    }
    return address;
  }

}

}