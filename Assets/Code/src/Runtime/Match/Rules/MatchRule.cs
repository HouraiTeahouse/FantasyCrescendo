using System;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public abstract class MatchRule : IInitializable<MatchConfig>, IDisposable,
                             ISimulation<MatchState, MatchInputContext> {
  protected MediatorContext Events { get; }

  protected MatchRule(MediatorContext context = null) {
    Events = context ?? Mediator.Global.CreateContext();
  }

  public virtual Task Initialize(MatchConfig config) => Task.CompletedTask;
  public virtual void Simulate(ref MatchState state,
                               in MatchInputContext input) {}
  public abstract MatchResolution? GetResolution(MatchState state);
  public abstract int GetWinner(MatchState state);

  public virtual void Dispose() => Events.Dispose();
}

}
