using System;
using System.Collections.Generic;
using System.Linq;

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

    internal Server(INetworkServer server, MatchConfig config) : base(server, config) {
      // TODO(james7132): Run server simulation for momentary state syncs
      NetworkServer.ReceivedInputs += OnRecievedInputs;
    }

    public override void Dispose() {
      NetworkServer.ReceivedInputs -= OnRecievedInputs;
    }

    void OnRecievedInputs(uint player, uint timestep,
                          IEnumerable<MatchInput> inputs) {
      NetworkServer.BroadcastInput(timestep, inputs);
    }

  }

  public sealed class Client : ClientGameController {

    MatchInput CurrentInput;
    MatchInput[] LocalInput;
    MatchInputContext InputContext;

    internal Client(INetworkClient client, MatchConfig config) : base(client, config) {
      LocalInput = new MatchInput[1];

      NetworkClient.OnRecievedInputs += OnRecievedInputs;
      NetworkClient.OnRecievedState += OnRecievedState;
    }

    public override void Update() {
      base.Update();
      NetworkClient.SendInput(Timestep, LocalInput);
      if (!CurrentInput.IsValid) return;
      InputContext.Update(CurrentInput);
      CurrentState = Simulation.Simulate(CurrentState, InputContext);
      var newInput = InputSource.SampleInput();
      LocalInput[0] = newInput;
      CurrentInput = newInput;
      Timestep++;
    }

    public override void Dispose() {
      NetworkClient.OnRecievedInputs -= OnRecievedInputs;
      NetworkClient.OnRecievedState -= OnRecievedState;
    }

    void OnRecievedInputs(uint timestep, IEnumerable<MatchInput> inputs) {
      //TODO(james7132): Cache/buffer inputs to smooth out gameplay
      if (timestep != Timestep) return;
      CurrentInput.MergeWith(inputs.First());
    }

    void OnRecievedState(uint timestep, MatchState state) {
      if (timestep < Timestep) return;
      CurrentState = state;
      Timestep = timestep;
    }

  }

}

}