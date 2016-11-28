using UnityEngine;

namespace HouraiTeahouse {

    public class ConstantRotation : MonoBehaviour {

        [SerializeField]
        Vector3 rotationPerSecond;

        public Vector3 RotationPerSecond {
            get { return rotationPerSecond; }
            set { rotationPerSecond = value; }
        }

        // Update is called once per frame
        void Update() { transform.Rotate(RotationPerSecond * Time.deltaTime); }

    }

}