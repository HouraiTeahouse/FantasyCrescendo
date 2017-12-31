using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo {

public class GameController : AbstractGameController {

  public IInputSource<GameInput> InputSource { get; set; }

  readonly GameInputContext inputContext;

  public GameController(GameConfig config) {
    inputContext = new GameInputContext(new GameInput(config));
  }

  public override void Update() {
    var input = InputSource.SampleInput();
    Assert.IsTrue(input.IsValid);
    inputContext.Update(input);
    CurrentState = Simulation.Simulate(CurrentState, inputContext);
  }

}

}
