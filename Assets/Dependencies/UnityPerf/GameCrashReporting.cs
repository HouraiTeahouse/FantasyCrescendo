using UnityEngine;
using UnityEngine.CrashLog;

/// <summary> Initializes Crash Reporting using Unity Game Performance Reporting. </summary>
public class GameCrashReporting : MonoBehaviour {
#if !UNITY_EDITOR
    /// <summary>
    /// Unity callback. Called on object instantiation.
    /// </summary>
    void Awake() {
		CrashReporting.Init(Application.cloudProjectId);
    }
#endif
}
