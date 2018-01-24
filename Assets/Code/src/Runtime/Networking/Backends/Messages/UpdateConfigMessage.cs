using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo {

public class ServerUpdateConfigMessage : MessageBase {
  public MatchConfig MatchConfig;
}

public class ClientUpdateConfigMessage : MessageBase {
  public PlayerConfig PlayerConfig;
}

}
