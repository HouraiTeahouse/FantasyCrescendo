using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo {

public class ServerStateMessage : MessageBase {

  public uint Timestamp;
  public GameState State;

}

}
