using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    [RequireComponent(typeof(CameraTarget))]
    public class MatchCameraTarget : MonoBehaviour {

        [SerializeField]
        float _cameraSpeed = 1f;

        [SerializeField]
        Vector2 _fovRange;

        [SerializeField]
        Vector2 _padding;

        [SerializeField]
        Vector3 _targetPositionBias;

        CameraTarget CameraTarget { get; set; }
        ICollection<Transform> Targets { get; set; }

        public Vector2 Padding {
            get { return _padding; }
            set { _padding = value; }
        }

        public Vector3 TargetPositionBias {
            get { return _targetPositionBias; }
            set { _targetPositionBias = value; }
        }

        void Awake() {
            Targets = new List<Transform>();
            CameraTarget = this.SafeGetComponent<CameraTarget>();
        }

        void Update() {
            var count = 0;
            float dt = Time.deltaTime;

            //Find the Bounds in which
            Vector3 sum = Vector3.zero;
            Vector3 min = Vector3.one * float.PositiveInfinity;
            Vector3 max = Vector3.one * float.NegativeInfinity;
            foreach (Transform target in Targets) {
                if (target == null || !target.gameObject.activeInHierarchy)
                    continue;
                count++;
                sum += target.position;
                min = Vector3.Min(min, target.position);
                max = Vector3.Max(max, target.position);
            }

            Vector3 targetPosition = _targetPositionBias + (count <= 0 ? Vector3.zero : sum / count);
            Vector2 size = (Vector2) max - (Vector2) min;

            // Calculate the actual padding to use
            var actualPadding = new Vector2(1 + 2 * _padding.x, 1 + 2 * _padding.y);

            // Compute Hadamard product between size and inverse padding to add the padding desired.
            size = new Vector2(size.x * actualPadding.x, size.y * actualPadding.y);

            // Calculate the target field of view for the proper cpuLevel of zoom
            float targetFOV = 2f * Mathf.Atan(size.x * 0.5f / Mathf.Abs(transform.position.z - targetPosition.z))
                * Mathf.Rad2Deg;

            // Clamp the FOV so it isn't too small or too big.
            targetFOV = Mathf.Clamp(targetFOV, _fovRange.x, _fovRange.y);

            // Keep the camera in the same Z plane.
            targetPosition.z = transform.position.z;

            // Lerp both the FOV and the position at the desired speeds
            CameraTarget.FOV = Mathf.Lerp(CameraTarget.FOV, targetFOV, dt * _cameraSpeed);
            transform.position = Vector3.Lerp(transform.position, targetPosition, dt * _cameraSpeed);
        }

        public void RegisterTarget(Transform target) { Targets.Add(target); }
        public void UnregisterTarget(Transform target) { Targets.Remove(target); }

    }

}
