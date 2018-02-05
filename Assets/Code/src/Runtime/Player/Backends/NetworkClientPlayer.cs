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
    using (var rental = ObjectPool<ServerUpdateConfigMessage>.Shared.Borrow()) {
      rental.RentedObject.MatchConfig = config;
      Connection.Send(MessageCodes.UpdateConfig, rental.RentedObject);
    }
  }

  public void StartMatch(MatchConfig config) {
    using (var rental = ObjectPool<MatchStartMessage>.Shared.Borrow()) {
      rental.RentedObject.MatchConfig = config;
      Connection.Send(MessageCodes.MatchStart, rental.RentedObject);
    }
  }

  public void SendInputs(uint timestamp, IEnumerable<MatchInput> inputs) {
    int size;
    var inputArray = ArrayUtil.ConvertToArray(inputs, out size);
    if (size <= 0) return;
    using (var rental = ObjectPool<InputSetMessage>.Shared.Borrow()) {
      var message = rental.RentedObject;
      message.StartTimestamp = timestamp;
      message.InputCount = (uint)size;
      message.Inputs = inputArray;
      Connection.Send(MessageCodes.UpdateInput, message, NetworkReliablity.Unreliable);
    }
  }

  public void Kick() => Connection?.Disconnect();

}

}