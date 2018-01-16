using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu(menuName = "Game Mode/Default Game Mode")]
public class DefaultGameMode : GameMode {

  public override async Task RunGame(MatchConfig config, bool loadStage = true) {
    var results = await new DefaultMatch().RunMatch(config, loadStage);
    await Config.Get<SceneConfig>().MatchEndScene.LoadAsync();
    var viewFactories = Object.FindObjectsOfType<AbstractViewFactory<PlayerMatchStats, PlayerConfig>>();
    await Task.WhenAll(results.PlayerStats.Select(p => BuildResultViews(p, viewFactories)));
  }

  async Task BuildResultViews(PlayerMatchStats playerStats, AbstractViewFactory<PlayerMatchStats, PlayerConfig>[] viewFactories) {
    var viewSets = await Task.WhenAll(viewFactories.Select(factory => factory.CreateViews(playerStats.Config)));
    foreach (var view in viewSets.SelectMany(v => v)) {
      view.ApplyState(playerStats);
    }
  }

}

}

