using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class NetworkClientPlayer {

  public readonly INetworkConnection Connection;
  public readonly byte PlayerID;
  public bool IsReady;
  public PlayerConfig Config;

  public NetworkClientPlayer(INetworkConnection connection, byte playerId) {
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

  public void Kick() => Connection?.Disconnect();

}

}