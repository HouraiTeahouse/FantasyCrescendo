using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu(menuName = "Game Mode/Default Game Mode")]
public class DefaultGameMode : GameMode {

  public override async Task RunGame(MatchConfig config, bool loadStage = true) {
    await new DefaultMatch().RunMatch(config, loadStage);
    await Config.Get<SceneConfig>().MatchEndScene.LoadAsync();
  }

}

}

