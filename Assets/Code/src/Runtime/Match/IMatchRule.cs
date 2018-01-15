using System;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public interface IMatchRule : IInitializable<MatchConfig>, IDisposable,
                              ISimulation<MatchState, MatchInputContext> {
  MatchResolution? GetResolution(MatchState state);
  uint? GetWinner(MatchState state);
}

}
