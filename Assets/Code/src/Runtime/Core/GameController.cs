using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// An IGameController implementation that runs a normal local game.
/// </summary>
public class GameController : IGameController<MatchState> {

  public uint Timestep { get; set; }
  public MatchState CurrentState { get; set; }
  public ISimulation<MatchState, MatchInputContext> Simulation { get; set; }
  public IInputSource<MatchInput> InputSource { get; set; }

  readonly MatchInputContext inputContext;

  public GameController(MatchConfig config) {
    inputContext = new MatchInputContext(new MatchInput(config));
  }

  public void Update() {
    var input = InputSource.SampleInput();
    Assert.IsTrue(input.IsValid);
    inputContext.Update(input);
    CurrentState = Simulation.Simulate(CurrentState, inputContext);
  }

}

}
