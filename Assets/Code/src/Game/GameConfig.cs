using System;
using System.Linq;
using System.Collections.Generic;

namespace HouraiTeahouse.FantasyCrescendo {

[Serializable]
public struct GameConfig : IValidatable {

  public PlayerConfig[] PlayerConfigs;

  public int PlayerCount => PlayerConfigs.Length;
  public bool IsValid => PlayerConfigs.IsAllValid();

}

}
