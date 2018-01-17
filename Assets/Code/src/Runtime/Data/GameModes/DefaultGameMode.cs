using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu(menuName = "Game Mode/Default Game Mode")]
public class DefaultGameMode : GameMode {

  public override async Task RunGame(MatchConfig config, bool loadStage = true) {
    var results = await new DefaultMatch().RunMatch(config, loadStage);
    await Config.Get<SceneConfig>().MatchEndScene.LoadAsync();
    var viewFactories = Object.FindObjectsOfType<ViewFactory<PlayerMatchStats, PlayerConfig>>();
    await Task.WhenAll(results.PlayerStats.Select(p => BuildResultViews(p, viewFactories)));
  }

  async Task BuildResultViews(PlayerMatchStats playerStats, 
                              ViewFactory<PlayerMatchStats, PlayerConfig>[] viewFactories) {
    var viewSets = await viewFactories.CreateViews(playerStats.Config);
    viewSets.ApplyState(playerStats);
  }

}

}

