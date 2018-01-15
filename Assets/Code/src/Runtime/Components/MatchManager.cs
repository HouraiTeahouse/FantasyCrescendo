using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class MatchManager : MonoBehaviour {

  public static MatchManager Instance { get; private set; }

  public MatchConfig Config;

  public IGameController<MatchState> MatchController;
  public IStateView<MatchState> View;

  TaskCompletionSource<MatchResult> MatchTask;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    Instance = this;
    enabled = false;
  }

  /// <summary>
  /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
  /// </summary>
  void FixedUpdate() => MatchController?.Update();

  /// <summary>
  /// Update is called every frame, if the MonoBehaviour is enabled.
  /// </summary>
  void Update() => View?.ApplyState(MatchController.CurrentState);

  public Task<MatchResult> RunMatch() {
    Debug.Log("Running match...");
    MatchTask = new TaskCompletionSource<MatchResult>();
    // TODO(james7132): Properly evaluate the match result here.
    return MatchTask.Task;
  }

  public void EndMatch(MatchResult result) {
    if (MatchTask == null || MatchTask.TrySetResult(result)) return;
    Debug.Log($"Match over. Winner: {result.WinningPlayerID}, Resolution {result.Resolution}");
  }

}

}
