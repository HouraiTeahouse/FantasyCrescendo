using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Stage {

    public class PauseCamera : MonoBehaviour {

        [SerializeField]
        CameraTarget _pauseCameraTarget;

        [SerializeField]
        Vector3 _startOffset;

        [SerializeField]
        Vector3 _translationSpeed = Vector3.one * 5f;

        [SerializeField]
        Vector2 _rotationSpeed = Vector2.one * 20f;

        CameraTarget _originalTarget;

        Quaternion _defaultRotation;
        float _defaultDistance;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Start() {
            _pauseCameraTarget = this.CachedGetComponent(_pauseCameraTarget, () => GetComponentInChildren<CameraTarget>());
            var timeManager = SmashTimeManager.Instance;
            if (timeManager == null) {
                Destroy(this);
                return;
            }
            _defaultRotation = transform.localRotation;
            _defaultDistance = _pauseCameraTarget.transform.localPosition.z;
            SmashTimeManager.OnPause  += OnPause;
            enabled = SmashTimeManager.Paused;
        } 

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy() {
            SmashTimeManager.OnPause -= OnPause;
        }

        void OnPause() {
            var timeManager = SmashTimeManager.Instance;
            if (timeManager == null)
                return;
            enabled = SmashTimeManager.Paused;
            if (enabled) {
                var player = SmashTimeManager.PausedPlayer;
                player = PlayerManager.Instance.MatchPlayers.Get(player.ID);
                if (player != null && player.PlayerObject != null) {
                    transform.position = player.PlayerObject.transform.position + _startOffset;
                    transform.localRotation = _defaultRotation;
                    _pauseCameraTarget.transform.localPosition = Vector3.forward * _defaultDistance;
                }
            }
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() {
            var player = SmashTimeManager.PausedPlayer;
            if (player == null)
                return;
            var controller = player.Controller;
            Vector3 translation = controller.DPad.Vector.Mult(_translationSpeed);
            Vector3 rotation = controller.LeftStick.Vector.Mult(_rotationSpeed);
            float inOut = 0f;
            if (controller.Action3) 
                inOut -= 1f;
            if (controller.Action4)
                inOut += 1f;
            var dt = Time.unscaledDeltaTime;
            rotation = new Vector3(rotation.y, -rotation.x);
            rotation *= dt;
            rotation += transform.eulerAngles;
            rotation.z = 0f;
            _pauseCameraTarget.transform.Translate(Vector3.forward * inOut * _translationSpeed.z, Space.Self);
            transform.Translate((Vector2)translation * dt, Space.World);
            transform.eulerAngles = rotation;
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable() {
            _originalTarget = CameraController.Instance.Target;
            CameraController.Instance.Target = _pauseCameraTarget;
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable() {
            if (_originalTarget != null)
                CameraController.Instance.Target = _originalTarget;
        }

    }

}
