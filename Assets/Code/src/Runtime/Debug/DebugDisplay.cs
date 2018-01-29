using System.Collections;
using System.Collections.Generic;
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

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    if (!Debug.isDebugBuild) {
      DestroyImmediate(this); 
    }
    TPSCounter = new FPSCounter();
  }

  /// <summary>
  /// Update is called every frame, if the MonoBehaviour is enabled.
  /// </summary>
  void FixedUpdate() {
    if (MatchManager == null) return;
    var currentTimestep = MatchManager.MatchController?.Timestep;
    if (currentTimestep != null && lastTimestep != currentTimestep) {
      TPSCounter.Update();
      lastTimestep = currentTimestep.Value;
    }
  }

  /// <summary>
  /// OnGUI is called for rendering and handling GUI events.
  /// This function can be called multiple times per frame (one call per event).
  /// </summary>
  void OnGUI() {
    var resolution = Screen.currentResolution;
    var screenRect = new Rect(0f, 0f, resolution.width, resolution.height);
    GUI.Label(screenRect, $"{TPSCounter.FPS:0.0}TPS");
  }

}

}