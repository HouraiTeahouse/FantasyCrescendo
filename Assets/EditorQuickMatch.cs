using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class EditorQuickMatch : MonoBehaviour {

  public GameManager GameManager;
  public GameMode GameMode;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    GameMode.CreateMatch().RunMatch(GameManager.Config, false);
  }

}

}
