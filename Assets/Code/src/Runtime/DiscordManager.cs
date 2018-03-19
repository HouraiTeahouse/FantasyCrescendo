using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;

namespace HouraiTeahouse.FantasyCrescendo {

public class DiscordManager : MonoBehaviour {

  const float kUpdateInterval = 15f / 2;

  public string ApplicationId;
  public bool AutoRegister;
  public string OptionalSteamID;

  DiscordRpc.EventHandlers eventHandlers;

  bool isReady = false;
  long startTimestamp;
  float updateTimer;

  /// <summary>
  /// Start is called on the frame when a script is enabled just before
  /// any of the Update methods is called the first time.
  /// </summary>
  void Start() {
    // startTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    updateTimer = float.MaxValue;
    eventHandlers.readyCallback += Ready;
    eventHandlers.disconnectedCallback += Disconnected;
    eventHandlers.errorCallback += Error;
    eventHandlers.joinCallback += Join;
    eventHandlers.spectateCallback += Spectate;
    eventHandlers.requestCallback += Request;
    DiscordRpc.Initialize(ApplicationId, ref eventHandlers, AutoRegister, OptionalSteamID);
    Debug.Log("Discord initalized!");
  }

  /// <summary>
  /// Update is called every frame, if the MonoBehaviour is enabled.
  /// </summary>
  void Update() {
    DiscordRpc.RunCallbacks();
    updateTimer += Time.deltaTime;
    if (updateTimer < kUpdateInterval) return;
    updateTimer = 0f;
    if (!isReady) return;
    DiscordRpc.UpdatePresence(new DiscordRpc.RichPresence {
      state = "In Menus",
      // details = "Test test test",
      startTimestamp = startTimestamp,
      largeImageKey = "default",
      largeImageText = "Large image text test",
      smallImageKey = "default",
      smallImageText = "Small image text test",
      partySize = (int)GameMode.GlobalMaxPlayers,
      partyMax = (int)GameMode.GlobalMaxPlayers
    });
  }

  /// <summary>
  /// Callback sent to all game objects before the application is quit.
  /// </summary>
  void OnApplicationQuit() {
    DiscordRpc.Shutdown();
    Debug.Log("Discord shutdown.");
  }

  void Ready() {
    isReady = true;
    Debug.Log("Discord ready!");
  }

  void Disconnected(int errorCode, string message) {
    isReady = false;
    Debug.LogWarning($"Discord disconnected: ({errorCode}) - {message}");
  }

  void Error(int errorCode, string message) {
    Debug.LogError($"Discord Error: ({errorCode}) - {message}");
  }

  void Join(string secret) {
    Debug.Log($"Discord Join. Secret: {secret}");
  }

  void Spectate(string secret) {
    Debug.Log($"Discord Spectate. Secret: {secret}");
  }

  void Request(ref DiscordRpc.JoinRequest request) {

  }

}

}