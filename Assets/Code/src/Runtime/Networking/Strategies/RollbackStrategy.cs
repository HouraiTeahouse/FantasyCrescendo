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
    readonly MatchInputContext inputContext;

    internal Server(INetworkServer server, MatchConfig config) : base(server, config) {
      // TODO(james7132): Run server simulation for momentary state syncs
      NetworkServer.ReceivedInputs += OnRecievedInputs;
      InputHistory = new InputHistory<MatchInput>();
      inputContext = new MatchInputContext(config);
    }

    public override void Update() {
      NetworkServer.BroadcastState(Timestep, CurrentState);
    }

    public override void Dispose() {
      NetworkServer.ReceivedInputs -= OnRecievedInputs;
    }

    void OnRecievedInputs(uint player, uint timestep,
                          IEnumerable<MatchInput> inputs) {
      InputHistory.DropBefore(Timestep);
      InputHistory.MergeWith(timestep, inputs);
      bool initialized = false;
      foreach (var input in InputHistory.TakeWhile(input => input.IsValid)) {
        // TODO(james7132): This should really take into account the prior inputs
        if (!initialized) {
          inputContext.Reset(input);
          initialized = true;
        } else {
          inputContext.Update(input);
        }
        CurrentState = Simulation.Simulate(CurrentState, inputContext);
        Timestep++;
      }
    }

  }

  public sealed class Client : ClientGameController {

    InputHistory<MatchInput> InputHistory;
    MatchInputContext InputContext;

    internal Client(INetworkClient client, MatchConfig config) : base(client, config) {
      NetworkClient.OnRecievedState += OnRecievedState;
      InputHistory = new InputHistory<MatchInput>();
      InputContext = new MatchInputContext(config);
    }

    public override void Update() {
      base.Update();
      var input = InputSource.SampleInput();
      if (InputHistory.Count > 0) {
        input = input.Predict(InputHistory.Latest);
      }
      InputHistory.Append(input);
      InputContext.Update(input);
      CurrentState = Simulation.Simulate(CurrentState, InputContext);
      NetworkClient.SendInput(InputHistory.LastConfirmedTimestep, InputHistory.Take(kMaxInputsSent));
    }

    public override void Dispose() {
      NetworkClient.OnRecievedState -= OnRecievedState;
    }

    void OnRecievedState(uint timestep, MatchState state) {
      if (timestep < Timestep) return;
      InputHistory.DropBefore(Timestep);
      bool reset = false;
      foreach (var input in InputHistory) {
        if (!reset) {
          InputContext.Reset(input);
          reset = true;
        } else {
          InputContext.Update(input);
        }
        state = Simulation.Simulate(state, InputContext);
      }
      CurrentState = state;
      Timestep = timestep;
    }

  }

}

}
