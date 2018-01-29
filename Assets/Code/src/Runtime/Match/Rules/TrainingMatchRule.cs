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

  public MatchState Simulate(MatchState state, MatchInputContext input) => state;

  public uint? GetWinner(MatchState state) => null;

  public MatchResolution? GetResolution(MatchState state) => null;

  public void Dispose() => Events?.Dispose();

  void OnPlayerDied(PlayerDiedEvent evt) => PlayerUtil.RespawnPlayer(evt);

}

}