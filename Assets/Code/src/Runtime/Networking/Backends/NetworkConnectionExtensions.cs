using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Networking = HouraiTeahouse.FantasyCrescendo.Networking.NetworkConnection;

namespace HouraiTeahouse.FantasyCrescendo.Networking {


public static class NetworkConnectionExtensions {

  public static void Send<T>(this NetworkConnection connection, byte header, 
                             in T message, NetworkReliablity reliablity = NetworkReliablity.Reliable) 
                             where T : INetworkSerializable {
    using (var writer = new Serializer()) {
      writer.Write(header);
      message.Serialize(writer);
      connection.SendBytes(writer.AsArray(), writer.Position, reliablity);
      (message as IDisposable)?.Dispose();
    }
  }

  public static void SendToAll<T>(this IEnumerable<NetworkConnection> connections, byte header,
                                  in T message, NetworkReliablity reliablity = NetworkReliablity.Reliable) 
                                  where T : INetworkSerializable {
    using (var writer = new Serializer()) {
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

}