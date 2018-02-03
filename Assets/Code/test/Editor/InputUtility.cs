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
      Buttons = (byte)random.Next(0, 255),
    };
  }

  public static void ForceValid(ref MatchInput input, int mask) {
    for (var i = 0; i < input.PlayerCount; i++) {
      input.PlayerInputs[i].IsValid = (mask & (1 << i)) != 0;
    }
  }

  public static void ForceValid(MatchInput[] inputs, int mask) {
    for (var i = 0; i < inputs.Length; i++) {
      ForceValid(ref inputs[i], mask);
    }
  }

  public static MatchInput RandomInput(int players) {
    var input = new MatchInput(players);
    for (var i = 0; i < input.PlayerCount; i++) {
      input.PlayerInputs[i] = RandomPlayerInput();
    }
    return input;
  }

  public static IEnumerable<MatchInput> RandomInput(int count, int players) {
    for (int i = 0; i < count; i++ ) {
      yield return RandomInput(players);
    }
  }

}
