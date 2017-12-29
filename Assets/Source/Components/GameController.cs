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
    simulation = new GameSimulation(Config);
    view = new GameView(Config);
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
