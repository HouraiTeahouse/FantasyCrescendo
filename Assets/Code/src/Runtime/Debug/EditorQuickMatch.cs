using HouraiTeahouse.FantasyCrescendo.Matches;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class EditorQuickMatch : EditorOnlyBehaviour {

  public MatchManager GameManager;
  public GameMode GameMode;
  public EditorMatchConfig Config;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  protected async override void Awake() {
    base.Awake();
    await GameMode.Execute(Config.CreateConfig(), false);
  }

}

}
