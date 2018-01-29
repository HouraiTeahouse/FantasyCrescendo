using HouraiTeahouse.FantasyCrescendo.Matches.Rules;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public class TrainingMatch : DefaultMatch {

  protected override IEnumerable<IMatchRule> CreateRules(MatchConfig config) {
    yield return new TrainingMatchRule();
  }

}

}