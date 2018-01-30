using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.Players;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class InputUtility {

  public static PlayerInput RandomPlayerInput() {
    return new PlayerInput {
      Movement = Random.insideUnitCircle,
      Smash = Vector2.zero,
      Buttons = (byte)Random.Range(0, 255),
    };
  }

  public static MatchInput RandomInput(int players) {
    return new MatchInput {
      PlayerInputs = Enumerable.Range(0, players).Select(_ => RandomPlayerInput()).ToArray()
    };
  }

  public static IEnumerable<MatchInput> RandomInput(int count, int players) {
    for (int i = 0; i < count; i++ ) {
      yield return RandomInput(players);
    }
  }

}
