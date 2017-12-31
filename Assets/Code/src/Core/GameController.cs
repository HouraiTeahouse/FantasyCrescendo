namespace HouraiTeahouse.FantasyCrescendo {

public class GameController : AbstractGameController {

  public IInputSource<GameInput> InputSource { get; set; }

  public override void Update() {
    CurrentState = Simulation.Simulate(CurrentState, InputSource.SampleInput());
  }

}

}
