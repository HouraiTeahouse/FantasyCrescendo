using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public struct NetworkDataMessage {

  public readonly INetworkConnection Connection;
  public readonly NetworkReader NetworkReader;

  public NetworkDataMessage(INetworkConnection connection, NetworkReader reader) {
    Connection = connection;
    NetworkReader = reader;
  }

  public T ReadAs<T>() where T : MessageBase, new() {
    var message = new T();
    message.Deserialize(NetworkReader);
    return message;
  }

}

}