using UnityEngine;

namespace Hourai.SmashBrew {
    
    [RequiredCharacterComponent]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class Gravity : HouraiBehaviour
    {

        [SerializeField]
        private float _gravity = 9.86f;

        public float GravityForce
        {
            get { return _gravity; }
            set { _gravity = Mathf.Abs(value); }
        }

        void FixedUpdate() {
            Rigidbody.AddForce(-Vector3.up * _gravity);
        }
    }

}