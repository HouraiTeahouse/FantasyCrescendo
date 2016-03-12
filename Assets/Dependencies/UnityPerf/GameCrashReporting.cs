using UnityEngine;
using UnityEngine.CrashLog;

/// <summary>
/// Initializes Crash Reporting using Unity Game Performance Reporting.
/// </summary>
public class GameCrashReporting : MonoBehaviour {
    /// <summary>
    /// The Unity Project ID for the project
    /// </summary>
    [SerializeField] private string _unityProjectID;

#if !UNITY_EDITOR
    /// <summary>
    /// Unity callback. Called on object instantiation.
    /// </summary>
    void Awake() {
		CrashReporting.Init(_unityProjectID);
    }
#endif
}