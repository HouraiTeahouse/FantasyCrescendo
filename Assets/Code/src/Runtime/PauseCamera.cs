using HouraiTeahouse.FantasyCrescendo.Matches;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;

namespace HouraiTeahouse.FantasyCrescendo {

public class PauseCamera : MonoBehaviour {

  public MatchManager MatchManager;
  public MatchPauseController PauseController;
  public CameraTarget CameraTarget;
  public Vector3 StartOffset;
  public Vector3 TranslationSpeed = Vector3.one * 5f;
  public Vector2 RotationSpeed = Vector2.one * 20f;
  //public InputControlType ZoomInControl = InputControlType.Action3;
  //public InputControlType ZoomOutControl  = InputControlType.Action4;

  MatchConfig MatchConfig;
  CameraTarget _originalTarget;
  Quaternion _defaultRotation;
  float _defaultDistance;
  int? pausedPlayerID;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Start() {
    if (CameraTarget == null) {
      CameraTarget = GetComponentInChildren<CameraTarget>();
    }
    _defaultRotation = transform.localRotation;
    _defaultDistance = CameraTarget.transform.localPosition.z;
    enabled = MatchManager.CurrentProgressionID == MatchProgressionState.Pause;
    var context = Mediator.Global.CreateUnityContext(this);
    context.Subscribe<MatchPauseStateChangedEvent>(OnPause);
    context.Subscribe<MatchStartEvent>(OnMatchStart);
  } 

  void OnMatchStart(MatchStartEvent evt) {
    MatchConfig = MatchManager.Config;
  }

  void OnPause(MatchPauseStateChangedEvent evt) {
    enabled = evt.IsPaused;
    if (enabled) {
      pausedPlayerID = evt.PausedPlayerID;
      var currentState = MatchManager.Instance.MatchController.CurrentState;
      var playerState = currentState.GetPlayerState(pausedPlayerID.Value);
      transform.position = (Vector3)playerState.Position + StartOffset;
      transform.localRotation = _defaultRotation;
      CameraTarget.transform.localPosition = Vector3.forward * _defaultDistance;
    } else {
      pausedPlayerID = null;
    }
  }

  /// <summary>
  /// Update is called every frame, if the MonoBehaviour is enabled.
  /// </summary>
  void Update() {
    if (pausedPlayerID == null) return;
    var localPlayerId = MatchConfig.PlayerConfigs[pausedPlayerID.Value].LocalPlayerID;
    var controller = Gamepad.all[localPlayerId];
    Vector3 translation = Vector3.Scale(controller.rightStick.ReadRawValue(), TranslationSpeed);
    Vector3 rotation = Vector3.Scale(controller.leftStick.ReadRawValue(), RotationSpeed);
    float inOut = 0f;
    if (controller.buttonWest.isPressed) inOut -= 1f;
    if (controller.buttonNorth.isPressed) inOut += 1f;
    var dt = Time.unscaledDeltaTime;
    rotation = new Vector3(rotation.y, -rotation.x) * dt;
    rotation += transform.eulerAngles;
    rotation.z = 0f;
    CameraTarget.transform.Translate(Vector3.forward * inOut * TranslationSpeed.z * dt, Space.Self);
    transform.Translate((Vector2)translation * dt, Space.World);
    transform.eulerAngles = rotation;
  }

  /// <summary>
  /// This function is called when the object becomes enabled and active.
  /// </summary>
  void OnEnable() {
    _originalTarget = CameraController.Instance.Target;
    CameraController.Instance.Target = CameraTarget;
  }

  /// <summary>
  /// This function is called when the behaviour becomes disabled or inactive.
  /// </summary>
  void OnDisable() {
    var cameraController = CameraController.Instance;
    if (_originalTarget != null && cameraController != null) {
      cameraController.Target = _originalTarget;
    }
  }

}

}