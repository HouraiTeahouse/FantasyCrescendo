using System;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A data object for configuring a single player within a multiplayer match.
/// </summary>
[Serializable]
public struct PlayerConfig : IValidatable {

  /// <summary>
  /// The Player ID of the player. Determines what is visually displayed
  /// to denote the player.
  /// </summary>
  public uint PlayerID;

  /// <summary>
  /// The local player number. Mainly used to determine what local input 
  /// device to read the input from.
  /// </summary>
  public uint LocalPlayerID;
  public PlayerSelection Selection;

  public bool IsValid => Selection.IsValid;

}

/// <summary>
/// A data object for managing the human selected elements of a player's
/// configuration.
/// </summary>
[Serializable]
public struct PlayerSelection : IValidatable {

  public uint CharacterID;
  public uint Pallete;
  
  // TODO(james7132): Properly implement
  public bool IsValid => true;

}

}