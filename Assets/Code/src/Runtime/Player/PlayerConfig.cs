using System;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A data object for configuring a single player within a multiplayer match.
/// </summary>
[Serializable]
public struct PlayerConfig : IValidatable {

  public uint PlayerID;
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