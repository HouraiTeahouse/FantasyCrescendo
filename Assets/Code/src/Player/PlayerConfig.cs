using System;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo {

[Serializable]
public struct PlayerConfig : IValidatable {

  public uint PlayerID;
  public PlayerSelection Selection;

  public bool IsValid => Selection.IsValid;

}

[Serializable]
public struct PlayerSelection : IValidatable {

  public uint CharacterID;
  public uint Pallete;
  
  // TODO(james7132): Properly implement
  public bool IsValid => true;

}

}
