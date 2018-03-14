using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

/// <summary>
/// An default IMatchController implementation that runs a normal local game.
/// </summary>
public class MatchController : IMatchController {

  public virtual uint Timestep { get; set; }
  public virtual MatchState CurrentState { get; set; }
  public virtual ISimulation<MatchState, MatchInputContext> Simulation { get; set; }
  public virtual IMatchInputSource InputSource { get; set; }

  readonly MatchInputContext inputContext;

  public MatchController(MatchConfig config) {
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
