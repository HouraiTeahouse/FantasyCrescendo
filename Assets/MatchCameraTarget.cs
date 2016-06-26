using UnityEngine;
using System.Collections.Generic;

namespace HouraiTeahouse.SmashBrew {
    
    [RequireComponent(typeof(CameraTarget))]
    public class MatchCameraTarget : EventHandlerBehaviour<PlayerSpawnEvent> {

        [SerializeField]
        float _cameraSpeed = 1f;

        [SerializeField]
        Vector2 _fovRange;

        [SerializeField]
        Vector2 _padding;

        [SerializeField]
        Vector3 _targetPositionBias;

        CameraTarget _target;
        HashSet<Transform> _targets;

        protected override void Awake() {
            base.Awake();
            _targets = new HashSet<Transform>();
            _target = GetComponent<CameraTarget>();
        }

        protected override void OnEvent(PlayerSpawnEvent eventArgs) {
            if (eventArgs == null || !eventArgs.PlayerObject)
                return;
            _targets.Add(eventArgs.PlayerObject.transform);
        }

        void Update() {
            var count = 0;
            float dt = DeltaTime;

            //Find the Bounds in which
            Vector3 sum = Vector3.zero;
            Vector3 min = Vector3.one * float.PositiveInfinity;
            Vector3 max = Vector3.one * float.NegativeInfinity;
            foreach (Transform target in _targets) {
                if (target == null || !target.gameObject.activeInHierarchy)
                    continue;
                count++;
                sum += target.position;
                min = Vector3.Min(min, target.position);
                max = Vector3.Max(max, target.position);
            }

            Vector3 targetPosition = _targetPositionBias
                + (count <= 0 ? Vector3.zero : sum / count);
            Vector2 size = (Vector2) max - (Vector2) min;

            // Calculate the actual padding to use
            var actualPadding = new Vector2(1 + 2 * _padding.x,
                1 + 2 * _padding.y);

            // Compute Hadamard product between size and inverse padding to add the padding desired.
            size = new Vector2(size.x * actualPadding.x,
                size.y * actualPadding.y);

            // Calculate the target field of view for the proper cpuLevel of zoom
            float targetFOV = 2f
                * Mathf.Atan(size.x * 0.5f
                    / Mathf.Abs(transform.position.z - targetPosition.z))
                * Mathf.Rad2Deg;

            // Clamp the FOV so it isn't too small or too big.
            targetFOV = Mathf.Clamp(targetFOV, _fovRange.x, _fovRange.y);

            // Keep the camera in the same Z plane.
            targetPosition.z = transform.position.z;

            // Lerp both the FOV and the position at the desired speeds
            _target.FOV = Mathf.Lerp(_target.FOV,
                targetFOV,
                dt * _cameraSpeed);
            transform.position = Vector3.Lerp(transform.position,
                targetPosition,
                dt * _cameraSpeed);
        }
    }
}

