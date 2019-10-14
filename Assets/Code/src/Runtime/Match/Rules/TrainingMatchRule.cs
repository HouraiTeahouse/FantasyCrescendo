using HouraiTeahouse.FantasyCrescendo.Players;
using System.Threading.Tasks;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

/// <summary>
/// Match rule for training mode. Players can die indefinitely, and a MatchEndd
/// event will not be fired without manual player intervention.
/// </summary>
public sealed class TrainingMatchRule : MatchRule {

  public TrainingMatchRule() : base() {
    Events.Subscribe<PlayerDiedEvent>(OnPlayerDied);
  }

  public override MatchResolution? GetResolution(MatchState state) => null;
  public override int GetWinner(MatchState state) => -1;

  void OnPlayerDied(PlayerDiedEvent evt) => PlayerUtil.RespawnPlayer(evt);

}

}
