using HouraiTeahouse.FantasyCrescendo.Matches;
using System.Collections.Generic;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class NetworkClientPlayer {

  public readonly NetworkConnection Connection;
  public readonly byte PlayerID;
  public bool IsReady;
  public PlayerConfig Config;

  public NetworkClientPlayer(NetworkConnection connection, byte playerId) {
    Connection = connection;
    PlayerID = playerId;
    IsReady = false;
    Config = new PlayerConfig();
  }

  public void SendConfig(MatchConfig config) {
    Connection.Send(MessageCodes.UpdateConfig, new ServerUpdateConfigMessage {
      MatchConfig = config 
    });
  }

  public void StartMatch(MatchConfig config) {
    Connection.Send(MessageCodes.MatchStart, new MatchStartMessage {
      MatchConfig = config 
    });
  }

  public void SendInputs(uint timestamp, IEnumerable<MatchInput> inputs) {
    int size;
    var inputArray = ArrayUtil.ConvertToArray(inputs, out size);
    if (size <= 0) return;
    Connection.Send(MessageCodes.UpdateInput, new InputSetMessage {
      StartTimestamp = timestamp,
      InputCount = (uint)size,
      ValidMask = MatchInput.AllValid,
      Inputs = inputArray,
    }, NetworkReliablity.Unreliable);
  }

  public void Kick() => Connection?.Disconnect();

}

}