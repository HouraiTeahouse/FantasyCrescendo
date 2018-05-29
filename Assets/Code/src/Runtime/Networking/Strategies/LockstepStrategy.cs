using HouraiTeahouse.FantasyCrescendo.Matches;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public sealed class LockstepStrategy : INetworkStrategy {

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

    MatchInput CurrentInput;
    MatchInput NextInput;
    MatchInput[] InputBuffer;

    internal Server(INetworkServer server, MatchConfig config) : base(server, config) {
      // TODO(james7132): Run server simulation for momentary state syncs
      NetworkServer.ReceivedInputs += OnRecievedInputs;
      CurrentInput = new MatchInput(config);
      NextInput = new MatchInput(config);
      InputBuffer = new MatchInput[1];
    }

    public override void Update() {
      base.Update();
      if (!CurrentInput.IsValid) return;
      InputBuffer[0] = CurrentInput;
      NetworkServer.BroadcastInput(Timestep, MatchInput.AllValid, InputBuffer);
    }

    public override void Dispose() {
      NetworkServer.ReceivedInputs -= OnRecievedInputs;
    }

    void OnRecievedInputs(int player, uint timestep,
                          ArraySegment<MatchInput> inputs) {
      if (timestep != Timestep) return;
      NextInput.MergeWith(inputs.Array[inputs.Offset]);
      if (!NextInput.IsValid) return;
      CurrentInput = NextInput;
      NextInput = CurrentInput;
      NextInput.Reset();
      Timestep++;
    }

  }

  public sealed class Client : ClientGameController {

    MatchInput CurrentInput;
    MatchInput[] LocalInput;
    MatchInputContext InputContext;

    internal Client(INetworkClient client, MatchConfig config) : base(client, config) {
      CurrentInput = new MatchInput(config);
      InputContext = new MatchInputContext(CurrentInput);
      NetworkClient.OnRecievedInputs += OnRecievedInputs;
      NetworkClient.OnRecievedState += OnRecievedState;
    }

    public override void Update() {
      base.Update();
      if (LocalInput == null) {
        LocalInput = new MatchInput[1];
        LocalInput[0] = InputSource.SampleInput();
      }
      NetworkClient.SendInput(Timestep, InputSource.ValidMask, LocalInput);
      if (!CurrentInput.IsValid) return;
      InputContext.Update(CurrentInput);

      var state = CurrentState;
      Simulation.Simulate(ref state, InputContext);
      CurrentState = state;

      LocalInput[0] = InputSource.SampleInput();
      CurrentInput.Reset();
      Timestep++;
    }

    public override void Dispose() {
      base.Dispose();
      NetworkClient.OnRecievedInputs -= OnRecievedInputs;
      NetworkClient.OnRecievedState -= OnRecievedState;
    }

    void OnRecievedInputs(uint timestep, ArraySegment<MatchInput> inputs) {
      //TODO(james7132): Cache/buffer inputs to smooth out gameplay
      if (timestep != Timestep + 1) return;
      var newInput = inputs.Array[inputs.Offset];
      Assert.IsTrue(newInput.IsValid);
      CurrentInput = newInput;
    }

    void OnRecievedState(uint timestep, MatchState state, MatchInput? latestInput) {
      if (timestep < Timestep) return;
      CurrentState = state;
      Timestep = timestep;
    }

  }

}

}