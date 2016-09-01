using UnityEngine;

namespace HouraiTeahouse {

    public sealed class CameraTarget : MonoBehaviour {

        [SerializeField]
        float _fov;

        public float FOV {
            get { return _fov; }
            set { _fov = value; }
        }

    }
    
}

