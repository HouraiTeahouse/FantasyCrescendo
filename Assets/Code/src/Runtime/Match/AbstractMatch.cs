using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace HouraiTeahouse.FantasyCrescendo {

public abstract class AbstractMatch {

  public async Task<MatchResult> RunMatch(GameConfig config, bool loadScene = true) {
    await DataLoader.LoadTask.Task;
    Task sceneLoad = Task.CompletedTask;
    if (loadScene) {
      var stage = Registry.Get<SceneData>().Get(config.StageID);
      Assert.IsTrue(stage != null && stage.IsStage);
      await stage.GameScene.LoadAsync();
    }
    var additionalScenes = Config.Get<StageConfig>().AdditionalStageScenes;
    await Task.WhenAll(additionalScenes.Select(s => s.LoadAsync(LoadSceneMode.Additive)));
    var gameManager = Object.FindObjectOfType<GameManager>();
    gameManager.Config = config;
    await InitializeMatch(gameManager, config);
    return await gameManager.RunMatch();
  }

  public abstract Task InitializeMatch(GameManager manager, GameConfig config);

}

public class DefaultMatch : AbstractMatch {

  public override async Task InitializeMatch(GameManager gameManager, GameConfig config) {
    var gameView = new GameView();
    var gameSim = new GameSimulation();
    var controller = new GameController(config);

    controller.CurrentState = CreateInitialState(config);
    controller.InputSource = new InControlInputSource(config);
    controller.Simulation = gameSim;

    gameManager.GameController = controller;
    gameManager.View = gameView;
    gameManager.enabled = false;

    var simTask = gameSim.Initialize(config).ContinueWith(task => {
      Debug.Log("Simulation initialized.");
    });
    var viewTask = gameView.Initialize(config).ContinueWith(task => {
      Debug.Log("View initialized.");
    });

    await Task.WhenAll(viewTask, simTask);
    controller.CurrentState = gameSim.ResetState(controller.CurrentState);
    gameManager.enabled = true;
    Debug.Log("Match initialized.");
  }

  GameState CreateInitialState(GameConfig config) {
    var initialState = new GameState(config);
    for (int i = 0; i < initialState.PlayerStates.Length; i++) {
      initialState.PlayerStates[i].Position = new Vector3(i * 2 - 3, 1, 0);
    }
    return initialState;
  }
}

}