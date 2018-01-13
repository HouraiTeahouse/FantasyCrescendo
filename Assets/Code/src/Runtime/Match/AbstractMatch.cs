using HouraiTeahouse.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo {

public abstract class AbstractMatch {

  public ITask<MatchResult> RunMatch(GameConfig config, bool loadScene = true) {
    ITask sceneLoad = Task.Resolved;
    if (loadScene) {
      var stage = Registry.Get<SceneData>().Get(config.StageID);
      Assert.IsTrue(stage != null && stage.IsStage);
      sceneLoad = stage.GameScene.LoadAsync();
    }
    return sceneLoad.Then(() => {
      var gameManager = Object.FindObjectOfType<GameManager>();
      gameManager.Config = config;
      return InitializeMatch(gameManager, config).Then(() => {
        return gameManager.RunMatch();
      });
    });
  }

  public abstract ITask InitializeMatch(GameManager manager, GameConfig config);

}

public class DefaultMatch : AbstractMatch {

  public override ITask InitializeMatch(GameManager gameManager, GameConfig config) {
    var gameView = new GameView();
    var gameSim = new GameSimulation();
    var controller = new GameController(config);

    controller.CurrentState = CreateInitialState(config);
    controller.InputSource = new InControlInputSource(config);
    controller.Simulation = gameSim;

    gameManager.GameController = controller;
    gameManager.View = gameView;
    gameManager.enabled = false;

    return Task.All(gameSim.Initialize(config), gameView.Initialize(config)).Then(() => {
      controller.CurrentState = gameSim.ResetState(controller.CurrentState);
      gameManager.enabled = true;
    });
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