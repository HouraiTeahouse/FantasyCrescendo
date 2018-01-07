using System;
using System.Linq;
using System.Collections.Generic;

namespace HouraiTeahouse.FantasyCrescendo {

[Serializable]
public struct GameConfig : IValidatable {

  public PlayerConfig[] PlayerConfigs;

  public bool IsValid => PlayerConfigs.IsAllValid();

}

}
