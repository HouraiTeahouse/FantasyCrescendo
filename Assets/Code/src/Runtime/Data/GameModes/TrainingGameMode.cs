using HouraiTeahouse.FantasyCrescendo.Matches;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu(menuName = "Game Modes/Training Game Mode")]
public class TrainingGameMode : GameMode {

  protected override async Task RunGame(MatchConfig config, bool loadStage = true) {
    await new TrainingMatch().RunMatch(config, loadStage);
    await Addressables.LoadScene(Config.Get<SceneConfig>().MainMenuScene);
  }

}

}

