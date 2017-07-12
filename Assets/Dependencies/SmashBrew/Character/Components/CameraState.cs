using HouraiTeahouse.SmashBrew.Stage;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Characters {

    [DisallowMultipleComponent]
    [AddComponentMenu("Smash Brew/Character/State/Camera State")]
    public class CameraState : MonoBehaviour {
        MatchCameraTarget _cameraTarget;

        void Awake() { _cameraTarget = FindObjectOfType<MatchCameraTarget>(); }

        void OnEnable() {
            if(_cameraTarget)
                _cameraTarget.RegisterTarget(transform);
        }

        void OnDisable() {
            if(_cameraTarget)
                _cameraTarget.UnregisterTarget(transform);
        }

    }

}

