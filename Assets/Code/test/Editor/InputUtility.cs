using HouraiTeahouse.FantasyCrescendo;
using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public static class InputUtility {
  
  static Random random = new Random();

  public static PlayerInput RandomPlayerInput() {
    return new PlayerInput {
      Movement = UnityEngine.Random.insideUnitCircle,
      Smash = random.NextDouble() > 0.5 ? UnityEngine.Random.insideUnitCircle : Vector2.zero,
      Buttons = (byte)random.Next(0, 31),
    };
  }

  public static MatchInput RandomInput(int players) {
    var input = new MatchInput();
    for (var i = 0; i < players; i++) {
      input[i] = RandomPlayerInput();
    }
    return input;
  }

  public static IEnumerable<MatchInput> RandomInput(int count, int players) {
    for (int i = 0; i < count; i++ ) {
      yield return RandomInput(players);
    }
  }

}
