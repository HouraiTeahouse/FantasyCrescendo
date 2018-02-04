using HouraiTeahouse.FantasyCrescendo.Matches;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class FPSCounter {

  public float FPS { get; private set; }

  float rollingDeltaTime;
  float lastTriggerTime;

  public FPSCounter() {
    lastTriggerTime = Time.unscaledTime;
  }

  public void Update() {
    var deltaTime = Mathf.Abs(Time.unscaledTime - lastTriggerTime);
    rollingDeltaTime += (deltaTime - rollingDeltaTime) * 0.1f;
    if (rollingDeltaTime != 0f) {
      FPS = 1.0f / rollingDeltaTime;
    }
    lastTriggerTime = Time.unscaledTime;
  }
  
}

public class DebugDisplay : MonoBehaviour {

  public MatchManager MatchManager;

  FPSCounter TPSCounter;
  uint lastTimestep;
  StringBuilder builder;

  bool showDisplay => MatchManager != null && 
                      MatchManager.isActiveAndEnabled && 
                      MatchManager.MatchController != null;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    if (!Debug.isDebugBuild) {
      DestroyImmediate(this); 
    }
    TPSCounter = new FPSCounter();
    builder = new StringBuilder();
  }

  /// <summary>
  /// Update is called every frame, if the MonoBehaviour is enabled.
  /// </summary>
  void FixedUpdate() {
    if (!showDisplay) return;
    var currentTimestep = MatchManager.MatchController.Timestep;
    if (lastTimestep != currentTimestep) {
      TPSCounter.Update();
      lastTimestep = currentTimestep;
    }
  }

  /// <summary>
  /// OnGUI is called for rendering and handling GUI events.
  /// This function can be called multiple times per frame (one call per event).
  /// </summary>
  void OnGUI() {
    if (!showDisplay) return;
    var state = MatchManager.MatchController.CurrentState;
    builder.Clear();
    builder.AppendLine($"{TPSCounter.FPS:0.0}TPS");
    for (uint i = 0; i < state.PlayerCount; i++) {
      var player = state.GetPlayerState(i);
      builder.AppendLine($"P{i+1}: S:{player.StateID} T:{player.StateTick}");
    }
    GUILayout.Label(builder.ToString());
  }

}

}