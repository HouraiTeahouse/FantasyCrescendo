// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections.Generic;
using HouraiTeahouse.Events;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    [RequireComponent(typeof(Camera))]
    public sealed class CameraController :
        EventHandlerBehaviour<PlayerSpawnEvent> {
        Camera _camera;

        [SerializeField]
        float _cameraSpeed = 1f;

        [SerializeField]
        Vector2 _fovRange;

        [SerializeField]
        Vector2 _padding;

        [SerializeField]
        Vector3 _targetPositionBias;

        HashSet<Transform> _targets;

        /// <summary> Unity callback. Called on object instantiation. </summary>
        protected override void Awake() {
            base.Awake();
            _camera = GetComponent<Camera>();
            _targets = new HashSet<Transform>();
        }

        protected override void OnEvent(PlayerSpawnEvent eventArgs) {
            if (eventArgs == null || !eventArgs.PlayerObject)
                return;
            _targets.Add(eventArgs.PlayerObject.transform);
        }

        /// <summary> Unity callback. Called once per frame after all Updates are processed. </summary>
        void LateUpdate() {
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
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView,
                targetFOV,
                dt * _cameraSpeed);
            transform.position = Vector3.Lerp(transform.position,
                targetPosition,
                dt * _cameraSpeed);
        }
    }
}