using HouraiTeahouse.FantasyCrescendo.Matches;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// An Editor-only component that makes it eaiser to read the game state information at runtime.
/// This component instantly destroys itself on instantiation in a non-Editor build.
/// </summary>
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