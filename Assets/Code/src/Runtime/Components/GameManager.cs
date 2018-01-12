using HouraiTeahouse.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo {

public class GameManager : NetworkBehaviour {

  public GameConfig Config;

  IGameController<GameState> gameController;
  IStateView<GameState> view;

  /// <summary>
  /// Start is called on the frame when a script is enabled just before
  /// any of the Update methods is called the first time.
  /// </summary>
  void Start() {
    var gameView = new GameView();
    var gameSim = new GameSimulation();
    var controller = new GameController(Config);

    controller.CurrentState = CreateInitialState();
    controller.InputSource = new InControlInputSource(Config);
    controller.Simulation = gameSim;

    Task.All(
      gameSim.Initialize(Config),
      gameView.Initialize(Config)).Then(() => {
        controller.CurrentState = gameSim.ResetState(controller.CurrentState);
        enabled = true;
      });

    enabled = false;

    gameController = controller;
    view = gameView;
  }

  GameState CreateInitialState() {
    var initialState = new GameState(Config);
    for (int i = 0; i < initialState.PlayerStates.Length; i++) {
      initialState.PlayerStates[i].Position = new Vector3(i * 2 - 3, 1, 0);
    }
    return initialState;
  }

  /// <summary>
  /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
  /// </summary>
  void FixedUpdate() {
    gameController.Update();
  }

  /// <summary>
  /// Update is called every frame, if the MonoBehaviour is enabled.
  /// </summary>
  void Update() {
    view.ApplyState(gameController.CurrentState);
  }

}

}
