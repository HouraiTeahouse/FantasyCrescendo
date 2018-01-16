using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class EditorQuickMatch : MonoBehaviour {

  public MatchManager GameManager;
  public GameMode GameMode;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  async void Awake() {
    await GameMode.RunGame(GameManager.Config, false);
  }

}

}
