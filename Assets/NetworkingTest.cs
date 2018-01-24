using HouraiTeahouse.FantasyCrescendo.Networking.UNET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class NetworkingTest : MonoBehaviour {

  public InputField IP;
  public InputField Port;

  NetworkHost host;
  NetworkGameClient client;
  INetworkInterface networkInterface;
  bool isInitialized = false;
  int timestamp;

  uint PortValue => uint.Parse(Port.text);

  public async void StartHost() {
    host = new NetworkHost(typeof(UNETNetworkInterface), new NetworkHostConfig());
    await host.Client.Connect("localhost", PortValue);
    host.Client.OnRecievedState += (t, state) => Debug.Log($"Time: {t} State: {state.PlayerCount}");
    isInitialized = true;
  }

  public async void StartClient() {
    client = new NetworkGameClient(typeof(UNETNetworkInterface), new NetworkClientConfig());
    await host.Client.Connect(IP.text, PortValue);
    client.OnRecievedState += (t, state) => Debug.Log($"Time: {t} State: {state.PlayerCount}");
    isInitialized = true;
  }

  /// <summary>
  /// Update is called every frame, if the MonoBehaviour is enabled.
  /// </summary>
  void Update() { 
    if (!isInitialized) return;
    if (host != null) {
      host.Server.BroadcastState((uint)timestamp++, new MatchState(2));
    }
    networkInterface?.Update();
  }

  /// <summary>
  /// This function is called when the MonoBehaviour will be destroyed.
  /// </summary>
  void OnDestroy() => networkInterface?.Dispose();

}

}