using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class MatchManager : MonoBehaviour {

  public static MatchManager Instance { get; private set; }

  public MatchConfig Config;

  public IGameController<MatchState> MatchController;
  public IStateView<MatchState> View;

  // TODO(james7132): Implement properly.
  public bool IsLocal => true;
  public bool IsPaused { get; private set; }

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
  void FixedUpdate() {
    if (!IsPaused) {
      MatchController?.Update();
    }
  }

  /// <summary>
  /// Update is called every frame, if the MonoBehaviour is enabled.
  /// </summary>
  void Update() => View?.ApplyState(MatchController.CurrentState);

  public Task<MatchResult> RunMatch() {
    Debug.Log("Running match...");
    MatchTask = new TaskCompletionSource<MatchResult>();
    Mediator.Global.Publish(new MatchStartEvent {
      MatchState = MatchController.CurrentState
    });
    // TODO(james7132): Properly evaluate the match result here.
    return MatchTask.Task;
  }

  public void SetPaused(bool paused) {
    if (!IsLocal) return;
    bool changed = paused != IsPaused;
    IsPaused = paused;
    if (changed) {
      Mediator.Global.Publish(new MatchPauseStateChangedEvent {
        MatchState = MatchController.CurrentState,
        Paused = paused
      });
    }
  }

  public async Task EndMatch(MatchResult result) {
    if (MatchTask == null || !MatchTask.TrySetResult(result)) return;
    Debug.Log($"Match over. Winner: {result.WinningPlayerID}, Resolution {result.Resolution}");
    await Mediator.Global.PublishAsync(new MatchEndEvent {
      MatchState = MatchController.CurrentState
    });
  }

}

}
