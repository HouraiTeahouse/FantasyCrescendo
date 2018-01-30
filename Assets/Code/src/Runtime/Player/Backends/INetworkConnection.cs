using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public interface INetworkConnection : IEntity {

  MessageHandlers MessageHandlers { get; }

  void SendBytes(byte[] buffer, int size, NetworkReliablity reachability = NetworkReliablity.Reliable);

  void Disconnect();
}

public static class INetworkConnectionExtensions {

  public static void Send(this INetworkConnection connection, byte header, 
                          MessageBase message, NetworkReliablity reliablity = NetworkReliablity.Reliable) {
    var writer = new NetworkWriter();
    writer.Write(header);
    message.Serialize(writer);
    connection.SendBytes(writer.AsArray(), writer.Position, reliablity);
    (message as IDisposable)?.Dispose();
  }

  public static void SendToAll(this IEnumerable<INetworkConnection> connections, byte header,
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