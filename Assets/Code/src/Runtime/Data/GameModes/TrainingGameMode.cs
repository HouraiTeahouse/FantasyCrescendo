using HouraiTeahouse.FantasyCrescendo.Matches;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu(menuName = "Game Modes/Training Game Mode")]
public class TrainingGameMode : GameMode {

  protected override async Task RunGame(MatchConfig config, bool loadStage = true) {
    await new TrainingMatch().RunMatch(config, loadStage);
    await Config.Get<SceneConfig>().MainMenuScene.LoadAsync();
  }

}

}

