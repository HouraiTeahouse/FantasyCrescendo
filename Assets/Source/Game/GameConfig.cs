using System;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo {

[Serializable]
public struct GameConfig {

  public PlayerConfig[] PlayerConfigs;

  public bool IsValid {
    get { return PlayerConfigs != null && PlayerConfigs.All(p => p.IsValid); }
  }

}

}
