using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo {

public class GameController : AbstractGameController {

  public IInputSource<GameInput> InputSource { get; set; }

  public override void Update() {
    var input = InputSource.SampleInput();
    Assert.IsTrue(input.IsValid);
    CurrentState = Simulation.Simulate(CurrentState, input);
  }

}

}
