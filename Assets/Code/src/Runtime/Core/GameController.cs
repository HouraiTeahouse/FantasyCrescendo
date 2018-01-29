using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// An IGameController implementation that runs a normal local game.
/// </summary>
public class GameController : IMatchController {

  public uint Timestep { get; set; }
  public virtual MatchState CurrentState { get; set; }
  public virtual ISimulation<MatchState, MatchInputContext> Simulation { get; set; }
  public virtual IInputSource<MatchInput> InputSource { get; set; }

  readonly MatchInputContext inputContext;

  public GameController(MatchConfig config) {
    inputContext = new MatchInputContext(new MatchInput(config));
  }

  public virtual void Update() {
    var input = InputSource.SampleInput();
    Assert.IsTrue(input.IsValid);
    inputContext.Update(input);
    CurrentState = Simulation.Simulate(CurrentState, inputContext);
    Timestep++;
  }

}

}
