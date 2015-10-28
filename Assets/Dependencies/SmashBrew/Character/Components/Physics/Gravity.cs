using UnityEngine;

namespace Hourai.SmashBrew {
    
    [RequiredCharacterComponent]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class Gravity : MonoBehaviour
    {

        [SerializeField]
        private float _gravity = 9.86f;

        private Rigidbody _rigidbody;

        public float GravityForce
        {
            get { return _gravity; }
            set { _gravity = Mathf.Abs(value); }
        }

        void Awake() {
            _rigidbody = GetComponent<Rigidbody>();
        }

        void FixedUpdate() {
            _rigidbody.AddForce(-Vector3.up * _gravity);
        }
    }

}