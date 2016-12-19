using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Characters {

    [DisallowMultipleComponent]
    public class CameraState : MonoBehaviour {
        MatchCameraTarget _cameraTarget;

        void Awake() { _cameraTarget = FindObjectOfType<MatchCameraTarget>(); }
        void OnEnable() { _cameraTarget.RegisterTarget(transform); }
        void OnDisable() { _cameraTarget.UnregisterTarget(transform); }

    }

}

