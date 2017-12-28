using System;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo {

[Serializable]
public struct PlayerConfig {

  public PlayerSelection Selection;

  public bool IsValid {
    get { return Selection.IsValid; }
  }

}

[Serializable]
public struct PlayerSelection {

  public uint CharacterID;
  public uint Pallete;

  public bool IsValid {
    get { return true; }
  }

}

}
