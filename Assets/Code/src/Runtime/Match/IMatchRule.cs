using System;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public interface IMatchRule : IInitializable<MatchConfig>, IDisposable,
                              ISimulation<MatchState, MatchInputContext> {
  MatchResolution? GetResolution(MatchState state);
  int GetWinner(MatchState state);
}

}
