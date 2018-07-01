using HouraiTeahouse.FantasyCrescendo.Matches;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class MatchStateDebugView : MonoBehaviour {

  public MatchManager Manager;
  public MatchState State;

#if !UNITY_EDITOR
  void Awake() => DestroyImmediate(this);
#endif

	// Update is called once per frame
	void Update () {
    if (Manager == null || Manager.MatchController == null) return;
    State = Manager.MatchController.CurrentState;
	}

}

}