using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using HouraiTeahouse.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public static class NetworkConnectionExtensions {

  public unsafe static void Send<T>(this NetworkConnection connection, byte header, 
                             in T message, NetworkReliablity reliablity = NetworkReliablity.Reliable) 
                             where T : INetworkSerializable {
    var buffer = stackalloc byte[SerializationConstants.kMaxMessageSize];
    var writer = Serializer.Create(buffer, 2048);
    writer.Write(header);
    message.Serialize(ref writer);
    connection.SendBytes(writer.ToArray(), writer.Position, reliablity);
    (message as IDisposable)?.Dispose();
  }

  public unsafe static void SendToAll<T>(this IEnumerable<NetworkConnection> connections, byte header,
                                  in T message, NetworkReliablity reliablity = NetworkReliablity.Reliable) 
                                  where T : INetworkSerializable {
    var buffer = stackalloc byte[SerializationConstants.kMaxMessageSize];
    var writer = Serializer.Create(buffer, 2048);
    writer.Write(header);
    message.Serialize(ref writer);
    var bufferSize = writer.Position;
    var buf = writer.ToArray();
    foreach (var connection in connections) {
      connection.SendBytes(buf, bufferSize, reliablity);
    }
    (message as IDisposable)?.Dispose();
  }

}

}