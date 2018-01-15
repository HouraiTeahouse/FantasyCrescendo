using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public abstract class GameMode : GameDataBase {
  public abstract Task RunGame(MatchConfig config, bool loadStage = true);
}

}
