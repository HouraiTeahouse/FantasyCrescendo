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
    readonly Dictionary<uint, uint> ClientTimesteps;

    uint StateSendTimer;
    MatchInput[] LatestInput;

    internal Server(INetworkServer server, MatchConfig config) : base(server, config) {
      // TODO(james7132): Run server simulation for momentary state syncs
      NetworkServer.ReceivedInputs += OnRecievedInputs;
      InputContext = new MatchInputContext(config);
      LatestInput = new MatchInput[1];
      LatestInput[0] = new MatchInput(config);
      InputHistory = new InputHistory<MatchInput>(LatestInput[0]);
      NetworkConfig = Config.Get<NetworkConfig>();
      ClientTimesteps = new Dictionary<uint, uint>();
      StateSendTimer = 0;
    }

    public override void Update() {
      StateSendTimer++;
      if (StateSendTimer < NetworkConfig.StateSendRate) return;
      LatestInput[0] = InputHistory.Newest.Input;
      Debug.Log($"Server: Broadcast State (Timestep: {Timestep})");
      NetworkServer.BroadcastState(Timestep, CurrentState);
      BroadcastInputs();
      StateSendTimer = 0;
    }

    public override void Dispose() {
      NetworkServer.ReceivedInputs -= OnRecievedInputs;
    }

    void OnRecievedInputs(uint player, uint timestep,
                          ArraySlice<MatchInput> inputs) {
      Debug.Log($"Server: Recieve Inputs {player} {timestep}");
      ForwardSimulate(timestep, inputs);
      UpdateClientTimestep(player, timestep);
    }

    void BroadcastInputs() {
      var timestamps = NetworkServer.Clients.Select(c => GetClientTimestamp(c.PlayerID));
      Debug.Log($"Server: Broadcast Inputs {string.Join(" ", timestamps)}");
      foreach (var client in NetworkServer.Clients) {
        var clientTimestep = GetClientTimestamp(client.PlayerID);
        var inputs = InputHistory.StartingWith(clientTimestep);
        var timestep = inputs.FirstOrDefault().Timestep;
        if (timestep < clientTimestep) continue;
        client.SendInputs(timestep, inputs.Select(i => i.Input).TakeWhile(i => i.IsValid));
      }
    }

    void ForwardSimulate(uint timestep, ArraySlice<MatchInput> inputs) {
      InputHistory.MergeWith(timestep, inputs);
      MatchInput input = InputHistory.Current.Input;
      InputContext.Reset(input);
      int count = 0; 
      var newestTimestep = InputHistory.Newest.Timestep;
      while (input.IsValid && InputHistory.Current.Timestep < newestTimestep) {
        count++;
        InputContext.Update(input);
        CurrentState = Simulation.Simulate(CurrentState, InputContext);
        input = InputHistory.Step();
      }
      Debug.Log($"Server: Forward Simulate (Timestep: {Timestep}) (Delta: {count})");
    }

    uint GetClientTimestamp(uint clientId) {
      uint timestamp = 0;
      ClientTimesteps.TryGetValue(clientId, out timestamp);
      return timestamp;
    }

    void UpdateClientTimestep(uint player, uint timestep) {
      ClientTimesteps[player] = Math.Max(GetClientTimestamp(player), timestep);
      var minTimestep = ClientTimesteps.Values.Min();
      InputHistory.DropBefore(minTimestep);
      Debug.Log($"Server: Update Client {timestep}");
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

    uint InputSendTimer;

    internal Client(INetworkClient client, MatchConfig config) : base(client, config) {
      NetworkClient.OnRecievedState += OnRecievedState;
      NetworkClient.OnRecievedInputs  += OnRecievedInputs;
      InputContext = new MatchInputContext(config);
      InputHistory = new InputHistory<MatchInput>(new MatchInput(config));
      NetworkConfig = Config.Get<NetworkConfig>();
      InputSendTimer = 0;
    }

    public override void Update() {
      base.Update();

      var input = InputSource.SampleInput();
      if (InputHistory.Current.Timestep == 0) {
        InputHistory.Current.Input.MergeWith(input);
      }
      input = InputHistory.Append(input);

      InputContext.Reset(InputHistory.Current.Input, input);
      InputContext.Predict();

      CurrentState = Simulation.Simulate(CurrentState, InputContext);
      
      InputSendTimer++;
      if (InputSendTimer < NetworkConfig.InputSendRate) return;
      var timestamp = InputHistory.Oldest.Timestep;
      var mask = InputSource.ValidMask;
      var inputs = InputHistory.Take(kMaxInputsSent).Select(i => i.Input);
      NetworkClient.SendInput(timestamp, mask, inputs);
      InputSendTimer = 0;
    }

    public override void Dispose() {
      NetworkClient.OnRecievedState -= OnRecievedState;
      NetworkClient.OnRecievedInputs -= OnRecievedInputs;
    }

    void OnRecievedInputs(uint timestep, ArraySlice<MatchInput> inputs) {
      InputHistory.MergeWith(timestep, inputs);
      Debug.Log($"Client: Update Inputs (Server: {timestep}) (Client Oldest: {InputHistory.Oldest.Timestep}) (Client Current: {Timestep})");
    }

    void OnRecievedState(uint timestep, MatchState state) {
      if (timestep < InputHistory.Oldest.Timestep) return;
      CurrentState = state;
      var start = timestep != 0 ? timestep - 1 : 0;
      foreach (var timedInput in InputHistory.StartingWith(start)) {
        Assert.IsTrue(timedInput.Timestep <= InputHistory.Current.Timestep);
        if (timedInput.Timestep < timestep || timedInput.Timestep == 0) {
          InputContext.Reset(timedInput.Input);
        } else {
          InputContext.Update(timedInput.Input);
          InputContext.Predict();
          CurrentState = Simulation.Simulate(CurrentState, InputContext);
        }
      }
      InputHistory.DropBefore(timestep);
      Debug.Log($"Client: Update State (Server: {timestep}) (Client Oldest: {InputHistory.Oldest.Timestep}) (Client Current: {Timestep})");
    }

  }

}

}
