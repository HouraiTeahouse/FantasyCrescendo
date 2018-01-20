using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class MatchRuleSimulation : IMatchSimulation {

  MatchConfig MatchConfig;
  readonly IMatchRule[] Rules;

  //TODO(james7132): Move this to somewhere more sane.
  MediatorContext Events;
  BlastZone BlastZone;

  public MatchRuleSimulation(IEnumerable<IMatchRule> rules) {
    Rules = rules.ToArray();
    foreach (var rule in Rules) {
      Debug.Log($"Match rule enabled: {rule.GetType().Name}");
    }
  }

  public Task Initialize(MatchConfig config) {
    MatchConfig = config;
    BlastZone = Object.FindObjectOfType<BlastZone>();
    return Task.WhenAll(Rules.Select(rule => rule.Initialize(config)));
  } 

  public MatchState Simulate(MatchState state, MatchInputContext input) {
    foreach (var rule in Rules) {
      state = rule.Simulate(state, input);
    }
    if (BlastZone != null) {
      state = BlastZone.Simulate(state);
    }
    CheckForFinish(state);
    return state;
  }

  public MatchState ResetState(MatchState state) => state;

  public void Dispose() => Events?.Dispose();

  void CheckForFinish(MatchState state) {
    MatchResolution? resolution = Rules.Select(r => r.GetResolution(state)).FirstOrDefault(res => res != null);
    if (resolution != null) {
      var winner = Rules.Select(r => r.GetWinner(state)).FirstOrDefault(w => w != null);
      if (MatchManager.Instance != null) {
        var result = CreateResult(state);
        result.Resolution = resolution.Value;
        result.WinningPlayerID = winner != null ? (int)winner : -1;
        MatchManager.Instance.EndMatch(result);
      }
    }
  }

  MatchResult CreateResult(MatchState state) {
    var players = new PlayerMatchStats[state.PlayerStates.Length];
    for (uint i = 0; i < players.Length; i++) {
      players[i].Config = MatchConfig.PlayerConfigs[i];
    }
    return new MatchResult {
      PlayerStats = players
    };
  }

}

}