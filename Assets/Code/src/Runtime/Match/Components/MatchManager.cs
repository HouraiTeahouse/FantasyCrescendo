using System;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public class MatchManager : MonoBehaviour, IDisposable {

  public static MatchManager Instance { get; private set; }

  [HideInInspector]
  public MatchConfig Config;

  public IMatchController MatchController;
  public IStateView<MatchState> View;

  public bool IsLocal => Config.IsLocal;

  public MatchProgressionState CurrentProgressionID {
    get => MatchController?.CurrentState?.StateID ?? default(MatchProgressionState);
    set => MatchController.CurrentState.StateID = value;
  }

  TaskCompletionSource<MatchResult> MatchTask;
  public bool IsPaused => CurrentProgressionID == MatchProgressionState.Pause;
  public int? PausedPlayer { get; private set; }

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
    if (MatchController != null && CurrentProgressionID != MatchProgressionState.Pause) {
      MatchController.Update();
    }
  }

  /// <summary>
  /// Update is called every frame, if the MonoBehaviour is enabled.
  /// </summary>
  void Update() => View?.UpdateView(MatchController.CurrentState);

  /// <summary>
  /// This function is called when the MonoBehaviour will be destroyed.
  /// </summary>
  void OnDestroy() => Dispose();

  public void Dispose() {
    View?.Dispose();
    MatchController?.Dispose();
  }

  public async Task<MatchResult> RunMatch() {
    if (MatchController == null) {
      throw new InvalidOperationException("Cannot run match without a match controller");
    }

    Debug.Log("Starting cooldown...");
    CurrentProgressionID = MatchProgressionState.Intro;
    await Mediator.Global.PublishAsync(new MatchStartCountdownEvent {
        MatchConfig = Config,
        MatchState = MatchController.CurrentState
    });
		
	 Debug.Log("Running match...");
    CurrentProgressionID = MatchProgressionState.InGame;
	 MatchTask = new TaskCompletionSource<MatchResult>();
    Mediator.Global.Publish(new MatchStartEvent {
      MatchConfig = Config,
      MatchState = MatchController.CurrentState
    });

    // TODO(james7132): Properly evaluate the match result here.
    var result = await MatchTask.Task;
    await Mediator.Global.PublishAsync(new MatchEndEvent {
      MatchConfig = Config,
      MatchState = MatchController.CurrentState
    });
    CurrentProgressionID = MatchProgressionState.End;
	 return result;
  }

  public void TogglePaused(int pausedPlayer) => SetPaused(!IsPaused, pausedPlayer);

  public void SetPaused(bool paused, int player) {
    if (!IsLocal || (IsPaused && player != PausedPlayer)) return;

    bool changed = IsPaused != paused;
    if (paused) {
        CurrentProgressionID = MatchProgressionState.Pause;
        PausedPlayer = player;
    } else {
        CurrentProgressionID = MatchProgressionState.InGame;
        PausedPlayer = null;
    }
    Assert.AreEqual(IsPaused, paused);
    if (changed) {
      Mediator.Global.Publish(new MatchPauseStateChangedEvent {
        MatchConfig = Config,
        MatchState = MatchController.CurrentState,
        IsPaused = paused,
        PausedPlayerID = player
      });
    }
  }

  public void ResetMatch(int player) {
      if (!IsLocal || player != PausedPlayer) return;
      // TODO(james7132): Properly construct this.
      EndMatch(new MatchResult{ 
          Resolution = MatchResolution.NoContest,
          WinningPlayerID = -1
      });
  }

  public void EndMatch(MatchResult result) {
    if (MatchTask == null || !MatchTask.TrySetResult(result)) return;
    Debug.Log($"Match over. Winner: {result.WinningPlayerID}, Resolution {result.Resolution}");
    Dispose();
  }

}

}
