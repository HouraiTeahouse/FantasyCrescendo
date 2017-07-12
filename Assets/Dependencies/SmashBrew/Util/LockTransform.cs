using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    public class LockTransform : MonoBehaviour {

        [SerializeField]
        Vector3 _position;
        [SerializeField]
        Vector3 _rotation;

        [SerializeField]
        bool _lockXPosition;
        [SerializeField]
        bool _lockYPosition;
        [SerializeField]
        bool _lockZPosition;
        [SerializeField]
        bool _lockXRotation;
        [SerializeField]
        bool _lockYRotation;
        [SerializeField]
        bool _lockZRotation;

        void Start() {
            if (!_lockXRotation && !_lockYRotation && !_lockZRotation)
                enabled = false;
        }
        
        void Update() {
            var euler = transform.eulerAngles;
            var newRotation = euler;
            if (_lockXRotation)
                euler.x = _rotation.x;
            if (_lockYRotation)
                euler.y = _rotation.y;
            if (_lockZRotation)
                euler.z = _rotation.z;
            if (newRotation != euler)
                transform.eulerAngles = euler;
        }

    }

}

