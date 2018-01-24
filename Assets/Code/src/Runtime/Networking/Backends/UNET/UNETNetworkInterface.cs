using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
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
  internal int HostID { get; set; }

  public event Action<INetworkConnection> OnPeerConnected;
  public event Action<INetworkConnection> OnPeerDisconnected;

  byte[] readBuffer;
  NetworkReader messageReader;

  public UNETNetworkInterface() {
    MessageHandlers = new MessageHandlers();
    Channels = new Dictionary<NetworkReliablity, int>();
    connectionMap = new Dictionary<int, UNETConnection>();
    connections = new List<INetworkConnection>();
    Connections = new ReadOnlyCollection<INetworkConnection>(connections);

    readBuffer = new byte[NetworkMessage.MaxMessageSize];
    messageReader = new NetworkReader(readBuffer);
  }

  public void Initialize(int port) {
    NetworkTransport.Init();

    var config = new ConnectionConfig();
    AddChannel(config, QosType.Reliable, NetworkReliablity.Reliable);
    AddChannel(config, QosType.StateUpdate, NetworkReliablity.StateUpdate);
    AddChannel(config, QosType.Unreliable, NetworkReliablity.Unreliable);

    var hostTopology = new HostTopology(config, (int)GameMode.GlobalMaxPlayers);

    HostID = NetworkTransport.AddHost(hostTopology, port);
  }

  public async Task<INetworkConnection> Connect(string address, int port) {
    address = ResolveAddress(address);

    byte error;
    var connectionId = NetworkTransport.Connect(HostID, address, port, 0, out error);
    UNETUtility.HandleError(error);

    INetworkConnection connection = null;
    var connectTask = new TaskCompletionSource<object>();
    Action<INetworkConnection> handler = (conn) => {
      if (conn.Id != connectionId) return;
      connection = conn;
      connectTask.TrySetResult(new object());
    };

    OnPeerConnected += handler;
    await connectTask.Task;
    OnPeerConnected -= handler;

    return connection;
  }

  internal int GetChannelID(NetworkReliablity reliability) {
    int channelId;
    if (Channels.TryGetValue(reliability, out channelId)) {
      return channelId;
    }
    return Channels[NetworkReliablity.Reliable];
  }

  public void Update() {
    if (HostID < 0) return;
    const int kMaxBufferSize = 1024;
    int connectionId, channelId, dataSize, bufferSize = kMaxBufferSize;
    byte error;
    NetworkEventType evt;
    do {
      evt = NetworkTransport.ReceiveFromHost(HostID, out connectionId, out channelId, readBuffer, bufferSize, out dataSize, out error);
      UNETUtility.HandleError(error);
      switch (evt) {
        case NetworkEventType.Nothing: break;
        case NetworkEventType.ConnectEvent: OnNewConnection(connectionId); break;
        case NetworkEventType.DataEvent: OnRecieveData(connectionId); break;
        case NetworkEventType.DisconnectEvent: OnDisconnect(connectionId); break;
        default:
          Debug.LogError($"Unkown network message type recieved: {evt}");
          break;
      }
    } while (evt != NetworkEventType.Nothing);
  }

  void OnNewConnection(int connectionId) {
    var connection = AddConnection(connectionId);
    OnPeerConnected?.Invoke(connection);
  }

  void OnRecieveData(int connectionId) {
    UNETConnection connection;
    if (!connectionMap.TryGetValue(connectionId, out connection)) return;
    messageReader.SeekZero();
    MessageHandlers.Execute(connection, messageReader);
  }

  void OnDisconnect(int connectionId) {
    UNETConnection connection;
    if (!connectionMap.TryGetValue(connectionId, out connection)) return;
    OnPeerDisconnected?.Invoke(connection);
    RemoveConnection(connection);
  }

  public void Dispose() => NetworkTransport.RemoveHost(HostID);

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