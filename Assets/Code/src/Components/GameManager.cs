using HouraiTeahouse.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo {

public class GameManager : NetworkBehaviour {

  public GameConfig Config;

  AbstractGameController gameController;
  IStateView<GameState> view;

  void Start() {
    var gameView = new GameView();
    var gameSim = new GameSimulation();
    var controller = new GameController();

    controller.CurrentState = CreateInitialState();
    controller.InputSource = new TestInputSource(Config);
    controller.Simulation = gameSim;

    Task.All(
      gameSim.Initialize(Config),
      gameView.Initialize(Config)).Then(() =>{
        Debug.Log("INITIALIZED");
      });

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

  void FixedUpdate() {
    gameController.Update();
  }

  void Update() {
    view.ApplyState(gameController.CurrentState);
  }

}

}
