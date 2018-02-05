using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public struct NetworkDataMessage {

  public readonly NetworkConnection Connection;
  public readonly Deserializer NetworkReader;

  public NetworkDataMessage(NetworkConnection connection, Deserializer reader) {
    Connection = connection;
    NetworkReader = reader;
  }

  public T ReadAs<T>() where T : INetworkSerializable, new() {
    var message = ObjectPool<T>.Shared.Rent();
    message.Deserialize(NetworkReader);
    return message;
  }

}

}