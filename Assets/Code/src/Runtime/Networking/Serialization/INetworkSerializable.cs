using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public interface INetworkSerializable {

  void Serialize(Serializer serializer);
  void Deserialize(Deserializer deserializer);

}

}