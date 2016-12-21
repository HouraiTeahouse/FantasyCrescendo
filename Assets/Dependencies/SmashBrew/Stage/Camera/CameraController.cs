using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    [RequireComponent(typeof(Camera))]
    public sealed class CameraController : MonoBehaviour {

        Camera _camera;

        [SerializeField]
        CameraTarget _target;

        public CameraTarget Target {
            get { return _target; }
            set { _target = value; }
        }

        /// <summary> Unity callback. Called on object instantiation. </summary>
        void Awake() { _camera = GetComponent<Camera>(); }

        /// <summary> Unity callback. Called once per frame after all Updates are processed. </summary>
        void LateUpdate() {
            if (_target == null)
                return;
            _camera.fieldOfView = _target.FOV;
            transform.Copy(_target.transform);
        }

    }

}