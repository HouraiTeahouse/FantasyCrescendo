using HouraiTeahouse.FantasyCrescendo.Matches;
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

    readonly InputHistory<MatchInput> InputHistory;
    readonly MatchInputContext InputContext;
    readonly NetworkConfig NetworkConfig;

    uint StateSendTimer;
    MatchInput[] LatestInput;

    internal Server(INetworkServer server, MatchConfig config) : base(server, config) {
      // TODO(james7132): Run server simulation for momentary state syncs
      NetworkServer.ReceivedInputs += OnRecievedInputs;
      InputHistory = new InputHistory<MatchInput>();
      InputContext = new MatchInputContext(config);
      LatestInput = new MatchInput[1];
      LatestInput[0] = new MatchInput(config);
      NetworkConfig = Config.Get<NetworkConfig>();
      StateSendTimer = 0;
    }

    public override void Update() {
      StateSendTimer++;
      if (StateSendTimer < NetworkConfig.StateSendRate) return;
      NetworkServer.BroadcastState(Timestep, CurrentState);
      NetworkServer.BroadcastInput(Timestep, LatestInput);
      StateSendTimer = 0;
    }

    public override void Dispose() {
      NetworkServer.ReceivedInputs -= OnRecievedInputs;
    }

    void OnRecievedInputs(uint player, uint timestep,
                          ArraySlice<MatchInput> inputs) {
      InputHistory.MergeWith(timestep, inputs);
      bool initialized = false;
      foreach (var input in InputHistory.TakeWhile(input => input.IsValid)) {
        // TODO(james7132): This should really take into account the prior inputs
        if (!initialized) {
          InputContext.Reset(input, input);
          initialized = true;
        } else {
          InputContext.Update(input);
        }
        CurrentState = Simulation.Simulate(CurrentState, InputContext);
        LatestInput[0] = input;
        Timestep++;
      }
      InputHistory.DropBefore(Timestep);
    }

  }

  public sealed class Client : ClientGameController {

    readonly InputHistory<MatchInput> InputHistory;
    readonly MatchInputContext InputContext;
    readonly NetworkConfig NetworkConfig;

    uint InputSendTimer;
    MatchInput[] LatestServerInput;

    internal Client(INetworkClient client, MatchConfig config) : base(client, config) {
      NetworkClient.OnRecievedState += OnRecievedState;
      NetworkClient.OnRecievedInputs  += OnRecievedInputs;
      InputHistory = new InputHistory<MatchInput>();
      InputContext = new MatchInputContext(config);
      LatestServerInput = new MatchInput[1];
      LatestServerInput[0] = new MatchInput(config);
      NetworkConfig = Config.Get<NetworkConfig>();
      InputSendTimer = 0;
    }

    public override void Update() {
      base.Update();
      var input = InputSource.SampleInput();
      input.Predict(LatestServerInput[0]);
      InputHistory.Append(input);
      InputContext.Update(input);
      CurrentState = Simulation.Simulate(CurrentState, InputContext);
      InputSendTimer++;
      if (InputSendTimer < NetworkConfig.InputSendRate) return;
      NetworkClient.SendInput(InputHistory.LastConfirmedTimestep, InputHistory.Take(kMaxInputsSent));
      InputSendTimer = 0;
    }

    public override void Dispose() {
      NetworkClient.OnRecievedState -= OnRecievedState;
      NetworkClient.OnRecievedInputs -= OnRecievedInputs;
    }

    void OnRecievedInputs(uint timestep, ArraySlice<MatchInput> inputs) {
      if (timestep < Timestep) return;
      LatestServerInput[0] = inputs[0];
    }

    void OnRecievedState(uint timestep, MatchState state) {
      if (timestep < Timestep) return;
      Timestep = timestep;
      InputHistory.DropBefore(Timestep);
      // Assert.AreEqual(InputHistory.LastConfirmedTimestep, timestep);
      bool reset = false;
      foreach (var input in InputHistory) {
        Assert.IsTrue(input.IsValid, "Stored input is not valid!");
        if (!reset) {
          // TODO(james7132): Properly start this off with the continuation of input.
          InputContext.Reset(input, input);
          reset = true;
        } else {
          InputContext.Update(input);
        }
        state = Simulation.Simulate(state, InputContext);
      }
      CurrentState = state;
    }

  }

}

}
