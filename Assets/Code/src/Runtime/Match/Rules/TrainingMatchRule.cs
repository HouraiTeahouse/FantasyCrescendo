using HouraiTeahouse.FantasyCrescendo.Players;
using System.Threading.Tasks;

namespace HouraiTeahouse.FantasyCrescendo.Matches.Rules {

/// <summary>
/// Match Rule for normal stock matches. Players have a limited number of lives.
/// After expending all lives, they will no longer respawn. Last player alive
/// will be declared the winner.
/// </summary>
public sealed class TrainingMatchRule : IMatchRule {

  MediatorContext Events;

  public Task Initialize(MatchConfig config) {
    Events = Mediator.Global.CreateContext();
    Events.Subscribe<PlayerDiedEvent>(OnPlayerDied);
    return Task.CompletedTask;
  }

  public void Simulate(ref MatchState state, in MatchInputContext input) {}

  public int GetWinner(MatchState state) => -1;

  public MatchResolution? GetResolution(MatchState state) => null;

  public void Dispose() => Events?.Dispose();

  void OnPlayerDied(PlayerDiedEvent evt) => PlayerUtil.RespawnPlayer(evt);

}

}