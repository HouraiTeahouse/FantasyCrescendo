using HouraiTeahouse.FantasyCrescendo.Matches;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class RollbackStrategy : INetworkStrategy {

  const int kMaxInputsSent = 60;
 
  public ServerGameController CreateServer(INetworkServer server,
                                           MatchConfig config) {
    return new Server(server, config);
  }

  public ClientGameController CreateClient(INetworkClient client,
                                           MatchConfig config) {
    return new Client(client, config);
  }

  // Server in lockstep only acts as a relay for inputs
  public sealed class Server : ServerGameController {

    public override uint Timestep {
      get { return InputHistory.Current.Timestep; }
      set { throw new NotSupportedException(); } 
    }

    readonly InputHistory<MatchInput> InputHistory;
    readonly MatchInputContext InputContext;
    readonly NetworkConfig NetworkConfig;
    readonly Dictionary<int, uint> ClientTimesteps;

    uint StateSendTimer;

    internal Server(INetworkServer server, MatchConfig config) : base(server, config) {
      NetworkServer.ReceivedInputs += OnRecievedInputs;
      NetworkServer.PlayerRemoved += OnRemovePlayer;
      InputContext = new MatchInputContext(config);
      InputHistory = new InputHistory<MatchInput>(new MatchInput(config));
      NetworkConfig = Config.Get<NetworkConfig>();
      ClientTimesteps = new Dictionary<int, uint>();
      StateSendTimer = 0;
    }

    public override void Update() {
      StateSendTimer++;
      ForwardSimulate();
      if (StateSendTimer < NetworkConfig.StateSendRate) return;
      NetworkServer.BroadcastState(Timestep, CurrentState, InputHistory.Current.Input);
      StateSendTimer = 0;
    }

    public override void Dispose() {
      NetworkServer.ReceivedInputs -= OnRecievedInputs;
      NetworkServer.PlayerRemoved -= OnRemovePlayer;
    }

    void OnRecievedInputs(int player, uint timestep,
                          ArraySegment<MatchInput> inputs) {
      InputHistory.MergeWith(timestep, inputs);
      UpdateClientTimestep(player, timestep);
    }

    void OnRemovePlayer(int playerId) {
      CurrentState[playerId].Stocks = sbyte.MinValue;
      Assert.IsTrue(!CurrentState[playerId].IsActive);
    }

    void BroadcastInputs() {
      foreach (var client in NetworkServer.Clients) {
        var clientTimestep = GetClientTimestamp(client.PlayerID);
        var inputs = InputHistory.StartingWith(clientTimestep);
        var timestep = inputs.FirstOrDefault().Timestep;
        if (timestep < clientTimestep) continue;
        client.SendInputs(timestep, inputs.Select(i => i.Input).TakeWhile(i => i.IsValid));
      }
    }

    void ForwardSimulate() {
      MatchInput input = InputHistory.Current.Input;
      var newestTimestep = InputHistory.Newest.Timestep;
      var state = CurrentState;
      while (input.IsValid && InputHistory.Current.Timestep < newestTimestep) {
        InputContext.Update(input);
        Simulation.Simulate(ref state, InputContext);
        input = InputHistory.Step();
      }
      CurrentState = state;
    }

    uint GetClientTimestamp(int clientId) {
      uint timestamp = 0;
      ClientTimesteps.TryGetValue(clientId, out timestamp);
      return timestamp;
    }

    void UpdateClientTimestep(int player, uint timestep) {
      ClientTimesteps[player] = Math.Max(GetClientTimestamp(player), timestep);
      uint minTimestep = uint.MaxValue;
      foreach (var clientTimestep in ClientTimesteps.Values) {
        if (clientTimestep < minTimestep) {
          minTimestep = clientTimestep;
        }
      }
      InputHistory.DropBefore(minTimestep);
    }

  }

  public sealed class Client : ClientGameController {

    public override uint Timestep {
      get { return InputHistory.Current.Timestep; }
      set { throw new NotSupportedException(); } 
    }

    readonly InputHistory<MatchInput> InputHistory;
    readonly MatchInputContext InputContext;
    readonly NetworkConfig NetworkConfig;

    uint latestServerStateTimestamp;
    MatchState latestServerState;
    MatchInput? latestServerInput;

    uint InputSendTimer;

    internal Client(INetworkClient client, MatchConfig config) : base(client, config) {
      NetworkClient.OnRecievedState += OnRecievedState;
      NetworkClient.OnRecievedInputs += OnRecievedInputs;
      InputContext = new MatchInputContext(config);
      InputHistory = new InputHistory<MatchInput>(new MatchInput(config));
      NetworkConfig = Config.Get<NetworkConfig>();
      InputSendTimer = 0;
      latestServerState = null;
    }

    public override void Update() {
      base.Update();
      FastForwardServerState();
      LocalSimulate();
      SendLocalInput();
    }

    public override void Dispose() {
      base.Dispose();
      NetworkClient.OnRecievedState -= OnRecievedState;
      NetworkClient.OnRecievedInputs -= OnRecievedInputs;
    }

    void FastForwardServerState() {
      if (latestServerState == null || latestServerStateTimestamp < InputHistory.Oldest.Timestep) return;
      CurrentState = latestServerState;
      var start = latestServerStateTimestamp != 0 ? latestServerStateTimestamp - 1 : 0;
      foreach (var timedInput in InputHistory.StartingWith(start)) {
        if (timedInput.Timestep >= InputHistory.Current.Timestep) break;
        var input = timedInput.Input;
        if (latestServerInput != null) {
          input.MergeWith(latestServerInput.Value);
        }
        if (timedInput.Timestep < latestServerStateTimestamp || timedInput.Timestep == 0) {
          InputContext.Reset(input);
        } else {
          InputContext.Update(input);
          InputContext.Predict();
          
          var state = CurrentState;
          Simulation.Simulate(ref state, InputContext);
          CurrentState = state;
        }
      }
      InputHistory.DropBefore(latestServerStateTimestamp);
      latestServerState = null;
    }

    void LocalSimulate() {
      var input = InputSource.SampleInput();
      var current = InputHistory.Current;
      if (current.Timestep == 0) {
        current.Input = current.Input.MergeWith(input);
        InputHistory.Current = current;
      }
      if (latestServerInput != null) {
        input = input.MergeWith(latestServerInput.Value, 
                                MatchInput.MergeStrategy.KeepValidity);
      }
      input = InputHistory.Append(input);

      InputContext.Reset(current.Input, input);
      InputContext.Predict();

      var state = CurrentState;
      Simulation.Simulate(ref state, InputContext);
      CurrentState = state;
    }

    void SendLocalInput() {
      InputSendTimer++;
      if (InputSendTimer < NetworkConfig.InputSendRate) return;
      var timestamp = InputHistory.Oldest.Timestep;
      var inputs = InputHistory.Take(kMaxInputsSent).Select(i => i.Input);
      NetworkClient.SendInput(timestamp, inputs);
      InputSendTimer = 0;
    }

    void OnRecievedInputs(uint timestep, ArraySegment<MatchInput> inputs) {
      InputHistory.MergeWith(timestep, inputs);
    }

    void OnRecievedState(uint timestep, MatchState state, MatchInput? latestInput) {
      if (timestep < latestServerStateTimestamp || state == null) return;
      latestServerStateTimestamp = timestep;
      latestServerState = state;
      latestServerInput = latestInput;
    }

  }

}

}
