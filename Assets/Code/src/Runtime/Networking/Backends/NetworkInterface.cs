﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UnityEngine;
using HouraiTeahouse.Compression;
using HouraiTeahouse.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public abstract class NetworkInterface : INetworkInterface {

  public MessageHandlers MessageHandlers { get; }
  public IReadOnlyCollection<NetworkConnection> Connections { get; }
  public NetworkInterfaceConfiguration Config { get; private set; }

  readonly IList<NetworkConnection> connections;
  readonly IDictionary<int, NetworkConnection> connectionMap;

  public event Action<NetworkConnection> OnPeerConnected;
  public event Action<NetworkConnection> OnPeerDisconnected;

  protected byte[] ReadBuffer;
  // Deserializer messageDeserializer;

  protected bool IsDisposed { get; private set; }

  protected NetworkInterface(int maxMsgSize) {
    MessageHandlers = new MessageHandlers();
    connectionMap = new Dictionary<int, NetworkConnection>();
    connections = new List<NetworkConnection>();
    Connections = new ReadOnlyCollection<NetworkConnection>(connections);
    ReadBuffer = ArrayPool<byte>.Shared.Rent(maxMsgSize);
    // messageDeserializer = new Deserializer(ReadBuffer);
  }

  public virtual Task Initialize(NetworkInterfaceConfiguration config) {
    Config = config;
    return Task.CompletedTask;
  }

  public abstract Task<NetworkConnection> Connect(NetworkConnectionConfig config);

  public abstract void Update();

  public abstract void Disconnect(int connectionId);

  public void Send(int connectionId, byte[] buffer, int count, 
                   NetworkReliablity reliability) {
    if (!connectionMap.ContainsKey(connectionId)) return;
    var pool = ArrayPool<byte>.Shared;
    var compressed = pool.Rent(count);
    var newSize = CLZF2.Compress(buffer, ref compressed, count);
    SendImpl(connectionId, compressed, newSize, reliability);
    pool.Return(compressed);
  }

  protected abstract void SendImpl(int connectionId, byte[] buffer, int count, NetworkReliablity reliablity);

  public virtual void Dispose() {
    if (IsDisposed) return;
    ArrayPool<byte>.Shared.Return(ReadBuffer);
    ReadBuffer = null;
    IsDisposed = true;
  }

  protected virtual void OnRecieveData(int connectionId, byte[] data, int dataSize) {
    NetworkConnection connection;
    if (!connectionMap.TryGetValue(connectionId, out connection)) return;
    var decompressed = ArrayPool<byte>.Shared.Rent(dataSize);
    CLZF2.Decompress(data, ref decompressed, dataSize);
    // messageDeserializer.Replace(decompressed);
    // messageDeserializer.SeekZero();
    // MessageHandlers.Execute(connection, messageDeserializer);
  }

  protected virtual void OnDisconnect(int connectionId, Exception exception) {
    NetworkConnection connection;
    if (!connectionMap.TryGetValue(connectionId, out connection)) return;
    connection.DisconnectInternal();
    OnPeerDisconnected?.Invoke(connection);
    RemoveConnection(connection);
  }

  protected virtual NetworkConnection OnNewConnection(int connectionId) {
    var connection = AddConnection(connectionId);
    if (connection.ConnectInternal()) {
      OnPeerConnected?.Invoke(connection);
    }
    return connection;
  }

  protected void RemoveConnection(NetworkConnection connection) {
    connectionMap.Remove(connection.Id);
    connections.Remove(connection);
  }

  public virtual ConnectionStats GetConnectionStats(int connectionId) => new ConnectionStats();

  protected NetworkConnection GetConnection(int connectionId) {
    NetworkConnection connection;
    if (connectionMap.TryGetValue(connectionId, out connection)) {
      return connection;
    }
    return null;
  }

  protected NetworkConnection AddConnection(int connectionId) {
    NetworkConnection connection;
    if (!connectionMap.TryGetValue(connectionId, out connection)) {
      connection = new NetworkConnection(this, connectionId);
      connectionMap[connectionId] = connection;
      connections.Add(connection);
    }
    return connection;
  }

}

}