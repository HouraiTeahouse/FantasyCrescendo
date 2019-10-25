using HouraiTeahouse.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class LobbyListUI : MonoBehaviour, IStateView<IEnumerable<Lobby>> {

  public RectTransform Container;
  public LobbyDisplay DisplayPrefab;

  List<LobbyDisplay> displays;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    displays = new List<LobbyDisplay>();
    if (Container == null) {
      Container = transform as RectTransform;
    }
  }

  /// <summary>
  /// This function is called when the MonoBehaviour will be destroyed.
  /// </summary>
  void OnDestroy() {
    foreach (var display in displays)  {
      Destroy(display);
    }
  }

  public void UpdateView(in IEnumerable<Lobby> state) {
    int index = 0;
    foreach (var lobby in state) {
      LobbyDisplay display;
      if (index >= displays.Count) {
        display = CreateDisplay();
        displays.Add(display);
      } else {
        display = displays[index];
      }
      var lobbyInfo = lobby;
      display.gameObject.SetActive(true);
      display.UpdateView(lobbyInfo);
      index++;
    }
    for (; index < displays?.Count; index++) {
      displays[index].gameObject.SetActive(false);
    }
  }

  public void Dispose() {
    ObjectUtil.Destroy(this);
    ObjectUtil.Destroy(Container);
  }

  LobbyDisplay CreateDisplay() {
    var display = Instantiate(DisplayPrefab);
    display.transform.SetParent(Container, false);
    return display;
  }

}

}