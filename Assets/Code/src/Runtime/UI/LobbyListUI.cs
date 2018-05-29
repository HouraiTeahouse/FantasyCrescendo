using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Matchmaking {

public class LobbyListUI : MonoBehaviour, IStateView<IEnumerable<LobbyInfo>> {

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

  public void ApplyState(ref IEnumerable<LobbyInfo> state) {
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
      display.ApplyState(ref lobbyInfo);
      index++;
    }
    for (; index < displays?.Count; index++) {
      displays[index].gameObject.SetActive(false);
    }
  }

  LobbyDisplay CreateDisplay() {
    var display = Instantiate(DisplayPrefab);
    display.transform.SetParent(Container, false);
    return display;
  }

}

}