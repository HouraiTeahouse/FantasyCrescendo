using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// An AbstractGameController implementation that runs a normal local game.
/// </summary>
public class GameController : IGameController<GameState> {

  public uint Timestep { get; set; }
  public GameState CurrentState { get; set; }
  public ISimulation<GameState, GameInputContext> Simulation { get; set; }
  public IInputSource<GameInput> InputSource { get; set; }

  readonly GameInputContext inputContext;

  public GameController(GameConfig config) {
    inputContext = new GameInputContext(new GameInput(config));
  }

  public void Update() {
    var input = InputSource.SampleInput();
    Assert.IsTrue(input.IsValid);
    inputContext.Update(input);
    CurrentState = Simulation.Simulate(CurrentState, inputContext);
  }

}

}
