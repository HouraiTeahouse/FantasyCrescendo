using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class CameraController : MonoBehaviour {

  public static CameraController Instance { get; set; }
  public CameraTarget Target;
  public Camera Camera;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    if (Camera == null) {
      Camera = GetComponentInChildren<Camera>();
    }
  }

  /// <summary>
  /// LateUpdate is called every frame, if the Behaviour is enabled.
  /// It is called after all Update functions have been called.
  /// </summary>
  void LateUpdate() {
    if (Target == null) return;
    var targetTransform = Target.transform;
    transform.position = targetTransform.position;
    transform.rotation = targetTransform.rotation;
    if (Camera != null) {
      Camera.fieldOfView = Target.FieldOfView;
    }
  }

}

}
