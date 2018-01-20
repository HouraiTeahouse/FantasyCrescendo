using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu(menuName = "Game Mode/Default Game Mode")]
public class DefaultGameMode : GameMode {

  protected override async Task RunGame(MatchConfig config, bool loadStage = true) {
    var results = await new DefaultMatch().RunMatch(config, loadStage);
    await Config.Get<SceneConfig>().MatchEndScene.LoadAsync();
    var viewFactories = Object.FindObjectsOfType<ViewFactory<PlayerMatchStats, PlayerConfig>>();
    await Task.WhenAll(results.PlayerStats.Select(p => BuildResultViews(p, viewFactories)));
  }

  public override bool IsValidConfig(MatchConfig config) {
    var selections = config.PlayerConfigs.Select(player => player.Selection);
    return base.IsValidConfig(config) && selections.Distinct().Count() == selections.Count();
  }

  async Task BuildResultViews(PlayerMatchStats playerStats, 
                              ViewFactory<PlayerMatchStats, PlayerConfig>[] viewFactories) {
    var viewSets = await viewFactories.CreateViews(playerStats.Config);
    viewSets.ApplyState(playerStats);
  }

}

}

