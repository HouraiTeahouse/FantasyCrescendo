using HouraiTeahouse.FantasyCrescendo.Matches;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public sealed class NetworkHostController : IMatchController {

  public uint Timestep {
    get { return ClientController.Timestep; }
    set {
      ClientController.Timestep = value;
      ServerController.Timestep = value;
    }
  }

  public MatchState CurrentState  {
    get { return ClientController.CurrentState; }
    set {
      ClientController.CurrentState = value;
      ServerController.CurrentState = value;
    }
  }

  public ISimulation<MatchState, MatchInputContext> Simulation {
    get { return ClientController.Simulation; }
    set {
      ClientController.Simulation = value;
      ServerController.Simulation = value;
    }
  }

  public IInputSource<MatchInput> InputSource {
    get { return ClientController.InputSource; }
    set {
      ClientController.InputSource = value;
      ServerController.InputSource = value;
    }
  }

  readonly ClientGameController ClientController;
  readonly ServerGameController ServerController;

  public NetworkHostController(ClientGameController clientController,
                               ServerGameController serverController) {
    ClientController = Argument.NotNull(clientController);
    ServerController = Argument.NotNull(serverController);
  }

  public void Update() {
    ServerController.Update();
    ClientController.Update();
  }

  public void Dispose() {
    ClientController.Dispose();
    ServerController.Dispose();
  }

}

}