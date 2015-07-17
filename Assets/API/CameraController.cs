using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Genso.API {

    [RequireComponent(typeof(Camera))]
    public sealed class CameraController : Singleton<CameraController> {

        private enum ZoomStyle { HeightBased, WidthBased }

        [SerializeField]
        private Vector3 targetPositionBias;

        [SerializeField]
        private Vector2 padding;

        [SerializeField]
        private float cameraSpeed = 1f;

        [SerializeField]
        private ZoomStyle zoomStyle;

        private Camera camera;
        private float fov;
        private List<Transform> targets;
        private Vector2 inversePadding;

        void Awake() {
            camera = GetComponent<Camera>();
            fov = camera.fieldOfView;
            targets = new List<Transform>();
            inversePadding = new Vector2(1 + 2 * padding.x, 1 + 2 * padding.y);
        }

        void LateUpdate() {
            int count = 0;
            
            //Find the Bounds in which
            Vector3 sum = Vector3.zero;
            Vector3 min = Vector3.one * float.PositiveInfinity;
            Vector3 max = Vector3.one * float.NegativeInfinity;
            foreach (Transform target in targets) {
                if (target == null)
                    continue;
                count++;
                sum += target.position;
                min = Vector3.Min(min, target.position);
                max = Vector3.Max(max, target.position);
            }
            Debug.Log(min);

            Vector3 targetPosition = targetPositionBias + ((count <= 0) ? Vector3.zero : sum / count);
            Vector2 size = (Vector2) max - (Vector2) min;

            transform.LookAt(targetPosition);

            // Compute Hadamard product between size and inverse padding to add the padding desired.
            size = new Vector2(size.x * inversePadding.x, size.y * inversePadding.y);

            if(zoomStyle == ZoomStyle.HeightBased)
                targetPosition.z -= 0.5f * size.y / Mathf.Tan(0.5f * Mathf.Deg2Rad * fov);
            else
                targetPosition.z -= 2f * size.x / Mathf.Tan(0.5f * Mathf.Deg2Rad * fov);

            transform.position = Vector3.Lerp(transform.position, targetPosition, Util.dt * cameraSpeed);
        }

        public static void AddTarget(Component target) {
            if(target == null)
                throw new ArgumentNullException("target");
            if(Instance == null)
                throw new InvalidOperationException("There is no CameraController instance in this scene.");
            Transform targetTransform = target.transform;
            if(!Instance.targets.Contains(targetTransform))
                Instance.targets.Add(targetTransform);
        }

        public static void AddTarget(GameObject target) {
            if (target == null)
                throw new ArgumentNullException("target");
            AddTarget(target.transform);
        }



    }

}
