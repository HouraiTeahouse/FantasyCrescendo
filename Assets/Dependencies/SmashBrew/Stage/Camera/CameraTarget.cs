using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Stage {

    [AddComponentMenu("Smash Brew/Stage/Camera Target")]
    public sealed class CameraTarget : MonoBehaviour {

        [SerializeField]
        float _fov;

        public float FOV {
            get { return _fov; }
            set { _fov = value; }
        }

    }

}
