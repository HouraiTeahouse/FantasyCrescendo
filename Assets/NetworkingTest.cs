using HouraiTeahouse.FantasyCrescendo.Networking.UNET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class NetworkingTest : MonoBehaviour {

  NetworkHost host;
  INetworkInterface networkInterface;
  bool isInitialized = false;
  int timestamp;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  async void Awake()  {
    networkInterface = new UNETNetworkInterface();
    host = new NetworkHost(networkInterface, new NetworkHostConfig());
    await networkInterface.Initialize();
    await host.Client.Connect("localhost", 8888);
    host.Client.ReceivedState += (t, state) => Debug.Log($"Time: {t} State: {state.PlayerCount}");
    isInitialized = true;
  }

  /// <summary>
  /// Update is called every frame, if the MonoBehaviour is enabled.
  /// </summary>
  void Update() { 
    if (!isInitialized) return;
    host.Server.BroadcastState((uint)timestamp++, new MatchState());
    networkInterface?.Update();
  }

  /// <summary>
  /// This function is called when the MonoBehaviour will be destroyed.
  /// </summary>
  void OnDestroy() => networkInterface.Dispose();

}

}