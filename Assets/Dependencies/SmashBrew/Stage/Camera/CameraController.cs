using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Stage {

    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Smash Brew/Stage/Camera/Camera Controller")]
    public sealed class CameraController : MonoBehaviour {

        Camera _camera;

        public static CameraController Instance { get; private set;}

        [SerializeField]
        CameraTarget _target;

        public CameraTarget Target {
            get { return _target; }
            set { _target = value; }
        }

        /// <summary> Unity callback. Called on object instantiation. </summary>
        void Awake() { 
            _camera = this.SafeGetComponent<Camera>(); 
            Instance = this;
        }

        /// <summary> Unity callback. Called once per frame after all Updates are processed. </summary>
        void LateUpdate() {
            if (_target == null)
                return;
            _camera.fieldOfView = _target.FOV;
            transform.Copy(_target.transform);
        }

    }

}
