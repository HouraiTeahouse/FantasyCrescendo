using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public enum ConnectionStatus {
  Initialized,
  Connecting,
  Connected,
  Disconnected
}

public struct ConnectionStats : IMergable<ConnectionStats> {
  public int TotalBytesOut;
  public int IncomingPacketsCount;
  public int IncomingPacketsLost;
  public int CurrentRTT;

  public float PacketLossPercent {
    get {
      var total = IncomingPacketsCount += IncomingPacketsLost;
      if (total == 0) return 0;
      return IncomingPacketsLost / total;
    }
  }

  public ConnectionStats MergeWith(ConnectionStats other) {
    TotalBytesOut += other.TotalBytesOut;
    IncomingPacketsCount += other.IncomingPacketsCount;
    IncomingPacketsLost += other.IncomingPacketsLost;
    CurrentRTT = Mathf.Max(CurrentRTT, other.CurrentRTT);
    return this;
  }
}

public abstract class NetworkConnection : IEntity {
  public abstract uint Id { get; }
  public bool IsConnected => Status == ConnectionStatus.Connected;
  public abstract ConnectionStatus Status { get; protected internal set; }
  public abstract ConnectionStats Stats { get; }
  public abstract MessageHandlers MessageHandlers { get; }
  public abstract void SendBytes(byte[] buffer, int size, NetworkReliablity reachability = NetworkReliablity.Reliable);
  public abstract void Disconnect();
}

public static class INetworkConnectionExtensions {

  public static void Send(this NetworkConnection connection, byte header, 
                          INetworkSerializable message, NetworkReliablity reliablity = NetworkReliablity.Reliable) {
    var writer = new Serializer();
    writer.Write(header);
    message.Serialize(writer);
    connection.SendBytes(writer.AsArray(), writer.Position, reliablity);
    (message as IDisposable)?.Dispose();
  }

  public static void SendToAll<T>(this IEnumerable<NetworkConnection> connections, byte header,
                                  T message, NetworkReliablity reliablity = NetworkReliablity.Reliable) 
                                  where T : INetworkSerializable {
    var writer = new Serializer();
    writer.Write(header);
    message.Serialize(writer);
    var bufferSize = writer.Position;
    var buffer = writer.AsArray();
    foreach (var connection in connections) {
      connection.SendBytes(buffer, bufferSize, reliablity);
    }
    (message as IDisposable)?.Dispose();
  }

  public static void Send(this NetworkConnection connection, byte header, 
                          MessageBase message, NetworkReliablity reliablity = NetworkReliablity.Reliable) {
    var writer = new NetworkWriter();
    writer.Write(header);
    message.Serialize(writer);
    connection.SendBytes(writer.AsArray(), writer.Position, reliablity);
    (message as IDisposable)?.Dispose();
  }

  public static void SendToAll(this IEnumerable<NetworkConnection> connections, byte header,
                               MessageBase message, NetworkReliablity reliablity = NetworkReliablity.Reliable) {
    var writer = new NetworkWriter();
    writer.Write(header);
    message.Serialize(writer);
    var bufferSize = writer.Position;
    var buffer = writer.AsArray();
    foreach (var connection in connections) {
      connection.SendBytes(buffer, bufferSize, reliablity);
    }
    (message as IDisposable)?.Dispose();
  }

}

}