using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class MatchCameraTarget : MonoBehaviour {

  public CameraTarget CameraTarget;
  public float SmoothTime = 0.5f;
  public Vector2 FoVRange;
  public float WidthPadding = 5f;
  public Vector3 TargetPositionBias;
  public float MaxSpreadDistance = 50f;
  public List<Transform> Targets;

  private Vector3 velocity;

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
  /// LateUpdate is called every frame, if the Behaviour is enabled.
  /// It is called after all Update functions have been called.
  /// </summary>
  void LateUpdate() {
    var bounds = GetTargetBounds();
    if (bounds == null) return;
    var center = bounds.Value.center;
    var width = bounds.Value.size.x + WidthPadding;

    center.z = transform.position.z;
    transform.position = Vector3.SmoothDamp(transform.position, center, ref velocity, SmoothTime);

    CameraTarget.FieldOfView = Mathf.Lerp(FoVRange.x, FoVRange.y, width / MaxSpreadDistance);
  }

  Bounds? GetTargetBounds() {
    bool found = false;
    var bounds = new Bounds();
    for (var i = 0; i < Targets.Count; i++) {
      if (Targets[i] == null) continue;
      var pos = Targets[i].position + TargetPositionBias;
      if (!found) {
        bounds = new Bounds(pos, Vector3.zero);
        found = true;
      } else {
        bounds.Encapsulate(pos);
      }
    }
    return found ? bounds : (Bounds?)null;
  }

  public void RegisterTarget(Transform target) => Targets.Add(target);
  public void UnregisterTarget(Transform target) => Targets.Remove(target);

}

}
