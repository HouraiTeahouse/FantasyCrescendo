using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.Networking;
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
    var tick = MatchManager.MatchController.Timestep;
    var state = MatchManager.MatchController.CurrentState;
    builder.Clear();
    builder.AppendLine($"{TPSCounter.FPS:0.0}TPS {GetEllapsedTime(tick)}");
    GetNetworkStats(tick);
    for (var i = 0; i < state.PlayerCount; i++) {
      var player = state[i];
      builder.AppendLine($"P{i+1}: S:{player.StateID} T:{player.StateTick}");
    }
    GUILayout.Label(builder.ToString());
  }

  string GetEllapsedTime(uint timestep) {
    var seconds = GetSeconds(timestep);
    var minutes = seconds / 60f;
    seconds %= 60f;
    return $"{minutes:00}:{seconds:00}";
  }

  float GetSeconds(float timestep) => timestep * Time.fixedDeltaTime;

  void GetNetworkStats(uint timestep) {
    var networkManager = NetworkManager.Instance;
    if (networkManager == null) return;
    var seconds = GetSeconds(timestep);
    // TODO(james7132): Properly set this up again
    // if (networkManager.IsClient) {
    //   builder.AppendLine($"Client: {NetworkStats(networkManager.Client.Connection.Stats, seconds)}");
    // }
    // if (networkManager.IsServer) {
    //   var stats = new ConnectionStats();
    //   foreach (var client in networkManager.Server.Clients) {
    //     stats.MergeWith(client.Connection.Stats);
    //   }
    //   builder.AppendLine($"Server: {NetworkStats(stats, seconds)}");
    // }
  }

  string NetworkStats(ConnectionStats stats, float seconds) {
    var bandwidth = GetHumanBandwdithDisplay(stats.TotalBytesOut);
    var bytesPerSecond = GetHumanBandwdithDisplay(stats.TotalBytesOut / seconds);
    return $"RTT: {stats.CurrentRTT}ms PL:{stats.PacketLossPercent:0}% B: {bandwidth} {bytesPerSecond}";
  }

  string GetHumanBandwdithDisplay(float count) {
    if (count < 1024) return count.ToString();
    count /= 1024;
    if (count < 1024) return $"{count:0.00}K";
    count /= 1024;
    if (count < 1024) return $"{count:0.00}M";
    count /= 1024;
    return $"{count:0.00}G";
  }

}

}