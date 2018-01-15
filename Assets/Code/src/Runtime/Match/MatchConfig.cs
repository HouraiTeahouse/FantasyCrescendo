using System;
using System.Linq;
using System.Collections.Generic;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A data object for configuring a game between multiple players.
/// </summary>
[Serializable]
public struct MatchConfig : IValidatable {

  public uint StageID;

  /// <summary>
  /// The number of stocks each player starts off with. If set to zero, the 
  /// match will not be a stock match.
  /// </summary>
  public uint Stocks;

  /// <summary>
  /// The amount of time the match will last for in ticks. If zero the game
  /// will not have a time limit.
  /// </summary>
  public uint Time;

  /// <summary>
  /// Individual configurations for each participating player.
  /// </summary>
  /// <remarks>
  /// Note that each player's player ID does not directly correspond with
  /// the array index for the player's config. All players in the game 
  /// with a valid configuration are assumed to be active. For example, 
  /// the player at index 1 may not be P2. Player 2 may be inactive and 
  /// the player may be P3 or P4 instead.
  /// </remarks>
  public PlayerConfig[] PlayerConfigs;

  /// <summary>
  /// Gets the number of participating players in the game.
  /// </summary>
  public int PlayerCount => PlayerConfigs.Length;

  public bool IsValid => PlayerConfigs.IsAllValid();

}

}
