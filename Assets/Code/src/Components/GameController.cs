using HouraiTeahouse.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class GameController : MonoBehaviour {

  public GameConfig Config;

  GameState state;
  InputHistory<GameInput> inputHistory;
  IInputSource<GameInput> inputSource;
  ISimulation<GameState, GameInput> simulation;
  IStateView<GameState> view;

  void Start() {
    inputSource = new TestInputSource(Config);
    state = CreateInitialState();

    var gameView = new GameView();
    var gameSim = new GameSimulation();

    Task.All(
      gameSim.Initialize(Config),
      gameView.Initialize(Config)).Then(() =>{
        Debug.Log("INITIALIZED");
      });

    simulation = gameSim;
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
    state = simulation.Simulate(state, inputSource.SampleInput());
  }

  void Update() {
    view.ApplyState(state);
  }

}

}
