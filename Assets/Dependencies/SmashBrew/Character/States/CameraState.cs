using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Characters {

    public class CameraState : MonoBehaviour {
        MatchCameraTarget _cameraTarget;

        void Awake() { _cameraTarget = FindObjectOfType<MatchCameraTarget>(); }
        void OnEnable() { _cameraTarget.RegisterTarget(transform); }
        void OnDisable() { _cameraTarget.UnregisterTarget(transform); }

    }

}

