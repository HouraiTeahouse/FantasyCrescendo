using HouraiTeahouse.FantasyCrescendo.Matches;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;

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

  public void FinishMatch(MatchResult result) {
    Connection.Send(MessageCodes.MatchFinish, new MatchFinishMessage {
      MatchResult = result
    });
  }

  public void SetServerReady(bool isReady) {
    Connection.Send(MessageCodes.ServerReady, new PeerReadyMessage {
      IsReady = isReady 
    });
  }

  public void SendInputs(uint timestamp, IEnumerable<MatchInput> inputs) {
    int size;
    var inputArray = ArrayUtil.ConvertToArray(inputs, out size);
    if (size <= 0) return;
    Connection.Send(MessageCodes.UpdateInput, new InputSetMessage {
      StartTimestamp = timestamp,
      InputCount = (uint)size,
      Inputs = inputArray,
    }, NetworkReliablity.Unreliable);
  }

  public void SendState(uint timestamp, MatchState state, MatchInput? latestInput) {
    Connection.Send(MessageCodes.UpdateState, new ServerStateMessage {
      Timestamp = timestamp,
      State = state,
      LatestInput = latestInput
    }, NetworkReliablity.Unreliable);
  }

  public void Kick() => Connection?.Disconnect();

}

}