using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class TrainingMatch : DefaultMatch {

  protected override IEnumerable<IMatchRule> CreateRules(MatchConfig config) {
    yield return new TrainingMatchRule();
  }

}

}