using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public abstract class GameMode : GameDataBase {
  public abstract AbstractMatch CreateMatch();
}

}
