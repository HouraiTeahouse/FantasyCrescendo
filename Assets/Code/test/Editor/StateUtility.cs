using HouraiTeahouse.FantasyCrescendo;
using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public static class StateUtility {

  static Random random = new Random();

  public static PlayerState RandomPlayerState() {
    return new PlayerState { 
      Position = random.Next() * new Vector2((float)random.NextDouble(), (float)random.NextDouble()),
      Velocity =random.Next() * new Vector2((float)random.NextDouble(), (float)random.NextDouble()),
      Direction = random.NextDouble() > 0.5,
      IsFastFalling= random.NextDouble() > 0.5,
      RemainingJumps = (uint)random.Next(10000),
      RespawnTimeRemaining = (uint)random.Next(10000),
      StateID = (uint)random.Next(10000),
      ShieldDamage = (uint)random.Next(10000),
      ShieldRecoveryCooldown = (uint)random.Next(10000),
      GrabbedLedgeID = (byte)random.Next(0, 255),
      Damage = random.Next(1000),
      Hitstun = (uint)random.Next(1000),
      Stocks = (sbyte)random.Next(-127, 127)
    };
  }

  public static MatchState RandomState(int players) {
    var state = new MatchState(players);
    for (uint i = 0; i < state.PlayerCount; i++) {
      state.SetPlayerState(i, RandomPlayerState());
    }
    return state;
  }

  public static IEnumerable<MatchState> RandomState(int count, int players) {
    for (int i = 0; i < count; i++ ) {
      yield return RandomState(players);
    }
  }

}
