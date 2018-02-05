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
  public IReadOnlyCollection<NetworkConnection> Connections { get; }

  readonly IList<NetworkConnection> connections;
  readonly IDictionary<int, UNETConnection> connectionMap;
  readonly IDictionary<NetworkReliablity, int> Channels;
  internal int HostID { get; set; }

  public event Action<NetworkConnection> OnPeerConnected;
  public event Action<NetworkConnection> OnPeerDisconnected;

  byte[] readBuffer;
  Deserializer messageDeserializer;
  bool disposed = false;

  public UNETNetworkInterface() {
    MessageHandlers = new MessageHandlers();
    Channels = new Dictionary<NetworkReliablity, int>();
    connectionMap = new Dictionary<int, UNETConnection>();
    connections = new List<NetworkConnection>();
    Connections = new ReadOnlyCollection<NetworkConnection>(connections);

    readBuffer = ArrayPool<byte>.Shared.Rent(NetworkMessage.MaxMessageSize);
    messageDeserializer = new Deserializer(readBuffer);
  }

  public void Initialize(uint port) {
    NetworkTransport.Init();

    var config = new ConnectionConfig();
    AddChannel(config, QosType.Reliable, NetworkReliablity.Reliable);
    AddChannel(config, QosType.Unreliable, NetworkReliablity.Unreliable);

    var hostTopology = new HostTopology(config, (int)GameMode.GlobalMaxPlayers);

    HostID = NetworkTransport.AddHost(hostTopology, (int)port);
  }

  public async Task<NetworkConnection> Connect(string address, int port) {
    address = ResolveAddress(address);

    byte error;
    var connectionId = NetworkTransport.Connect(HostID, address, port, 0, out error);
    UNETUtility.HandleError(error);

    var connection = AddConnection(connectionId);
    await connection.ConnectTask.Task;
    return connection;
  }

  internal int GetChannelID(NetworkReliablity reliability) {
    int channelId;
    if (Channels.TryGetValue(reliability, out channelId)) {
      return channelId;
    }
    return Channels[NetworkReliablity.Reliable];
  }

  public void Send(int connectionId, byte[] buffer, int count, 
                   NetworkReliablity reliability) {
    if (!connectionMap.ContainsKey(connectionId)) return;
    var pool = ArrayPool<byte>.Shared;
    var compressed = pool.Rent(count);
    var newSize = CLZF2.Compress(buffer, ref compressed, count);
    byte error;
    var channelId = GetChannelID(reliability);
    NetworkTransport.Send(HostID, connectionId, channelId, compressed, newSize, out error);
    UNETUtility.HandleError(error);
    pool.Return(compressed);
  }

  public void Update() {
    if (HostID < 0) return;
    int connectionId, channelId, dataSize, bufferSize = NetworkMessage.MaxMessageSize;
    byte error;
    NetworkEventType evt;
    do {
      evt = NetworkTransport.ReceiveFromHost(HostID, out connectionId, out channelId, readBuffer, bufferSize, out dataSize, out error);
      switch (evt) {
        case NetworkEventType.Nothing: break;
        case NetworkEventType.ConnectEvent: OnNewConnection(connectionId); break;
        case NetworkEventType.DataEvent: OnRecieveData(connectionId, dataSize); break;
        case NetworkEventType.DisconnectEvent: OnDisconnect(connectionId, error); break;
        default:
          Debug.LogError($"Unkown network message type recieved: {evt}");
          break;
      }
    } while (evt != NetworkEventType.Nothing && !disposed);
  }

  void OnNewConnection(int connectionId) {
    var connection = AddConnection(connectionId);
    connection.Status = ConnectionStatus.Connected;
    connection.ConnectTask.TrySetResult(new object());
    OnPeerConnected?.Invoke(connection);
  }

  void OnRecieveData(int connectionId, int dataSize) {
    UNETConnection connection;
    if (!connectionMap.TryGetValue(connectionId, out connection)) return;
    var decompressed = ArrayPool<byte>.Shared.Rent(dataSize);
    CLZF2.Decompress(readBuffer, ref decompressed, dataSize);
    messageDeserializer.Replace(decompressed);
    messageDeserializer.SeekZero();
    MessageHandlers.Execute(connection, messageDeserializer);
  }

  void OnDisconnect(int connectionId, byte error) {
    UNETConnection connection;
    if (!connectionMap.TryGetValue(connectionId, out connection)) return;
    connection.Status = ConnectionStatus.Disconnected;
    var exception = UNETUtility.CreateError(error);
    if (exception == null) {
      connection.ConnectTask.TrySetResult(new object());
    } else {
      connection.ConnectTask.TrySetException(exception);
    }
    OnPeerDisconnected?.Invoke(connection);
    RemoveConnection(connection);
  }

  public void Dispose() {
    if (disposed) return;
    NetworkTransport.RemoveHost(HostID);
    ArrayPool<byte>.Shared.Return(readBuffer);
    disposed = true;
    readBuffer=  null;
  }

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