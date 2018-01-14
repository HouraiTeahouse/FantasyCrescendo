using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class MatchCameraTarget : MonoBehaviour {

  public CameraTarget CameraTarget;
  public float CameraSpeed = 1f;
  public Vector2 FoVRange;
  public Vector2 Padding;
  public Vector3 TargetPositionBias;
  public List<Transform> Targets;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    Targets = Targets ?? new List<Transform>();
    if (CameraTarget == null) {
      CameraTarget = GetComponentInChildren<CameraTarget>();
    }
  }

  /// <summary>
  /// Update is called every frame, if the MonoBehaviour is enabled.
  /// </summary>
  void Update() {
    var count = 0;
    float dt = Time.deltaTime;

    //Find the Bounds in which
    Vector3 sum = Vector3.zero;
    Vector3 min = Vector3.one * float.PositiveInfinity;
    Vector3 max = Vector3.one * float.NegativeInfinity;
    foreach (Transform target in Targets) {
      if (target == null || !target.gameObject.activeInHierarchy) {
        continue;
      }
      count++;
      sum += target.position;
      min = Vector3.Min(min, target.position);
      max = Vector3.Max(max, target.position);
    }

    Vector3 targetPosition = TargetPositionBias + (count <= 0 ? Vector3.zero : sum / count);
    Vector2 size = (Vector2) max - (Vector2) min;

    // Calculate the actual padding to use
    var actualPadding = new Vector2(1 + 2 * Padding.x, 1 + 2 * Padding.y);

    // Compute Hadamard product between size and inverse padding to add the padding desired.
    size = new Vector2(size.x * actualPadding.x, size.y * actualPadding.y);

    // Calculate the target field of view for the proper cpuLevel of zoom
    float targetFOV = 2f * Mathf.Atan(size.x * 0.5f / Mathf.Abs(transform.position.z - targetPosition.z))
        * Mathf.Rad2Deg;

    // Clamp the FOV so it isn't too small or too big.
    targetFOV = Mathf.Clamp(targetFOV, FoVRange.x, FoVRange.y);

    // Keep the camera in the same Z plane.
    targetPosition.z = transform.position.z;

    // Lerp both the FOV and the position at the desired speeds
    CameraTarget.FieldOfView = Mathf.Lerp(CameraTarget.FieldOfView, targetFOV, Time.deltaTime * CameraSpeed);
    transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * CameraSpeed);
}

  public void RegisterTarget(Transform target) => Targets.Add(target);
  public void UnregisterTarget(Transform target) => Targets.Remove(target);

}

}
