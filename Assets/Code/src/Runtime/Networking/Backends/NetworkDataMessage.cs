using UnityEngine;
using HouraiTeahouse.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public struct NetworkDataMessage {

  public readonly NetworkConnection Connection;
  public Deserializer NetworkReader;

  public NetworkDataMessage(NetworkConnection connection, Deserializer reader) {
    Connection = connection;
    NetworkReader = reader;
  }

  public T ReadAs<T>() where T : INetworkSerializable, new() {
    var message = ObjectPool<T>.Shared.Rent();
    message.Deserialize(ref NetworkReader);
    return message;
  }

}

}