using HouraiTeahouse.FantasyCrescendo.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo {
    
public class NetworkMenu : MonoBehaviour {

  public InputField IP;
  public InputField Port;

  public GameObject NetworkMenuScreen;
  public GameObject ConnectingScreen;
  public GameObject SuccessScreen;
  public GameObject ErrorScreen;
  public Text ErrorText;

  int PortValue => int.Parse(Port.text);

  public async void StartHost() {
    var networkManager = NetworkManager.Instance;
    var hostConfig = new NetworkHostConfig {
      ServerConfig = new NetworkServerConfig  {
        Port = PortValue
      }
    };
    Debug.Log($"Started host on {PortValue}");
    try {
      SetActive(ConnectingScreen);
      await networkManager.StartHost(hostConfig);
    } catch (NetworkingException exception) {
      networkManager.StopHost();
      SetActive(ErrorScreen);
      ErrorText.text = exception.Message;
      return;
    }
    SetActive(SuccessScreen);
  }

  public async void StartClient() {
    var networkManager = NetworkManager.Instance;
    var clientConfig = new NetworkClientConfig();
    var client = networkManager.StartClient(clientConfig);
    Debug.Log($"Connecting to {IP.text}:{PortValue}");
    try {
      SetActive(ConnectingScreen);
      await client.Connect(
        new NetworkConnectionConfig {
          IP = IP.text,
          Port = PortValue
        });
    } catch (NetworkingException exception) {
      networkManager.StopClient();
      SetActive(ErrorScreen);
      ErrorText.text = exception.Message;
      Debug.LogError(exception);
      return;
    }
    SetActive(SuccessScreen);
  }

  public void ReturnToMainScreen() {
    SetActive(NetworkMenuScreen);
  }

  void SetActive(GameObject gameObj) {
    foreach (var screen in new[] {NetworkMenuScreen, ConnectingScreen, SuccessScreen, ErrorScreen}) {
      ObjectUtil.SetActive(screen, screen == gameObj);
    }
  }

  /// <summary>
  /// This function is called when the object becomes enabled and active.
  /// </summary>
  void OnEnable() => SetActive(NetworkMenuScreen);

}

}