using UnityEngine;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    public class CharacterGravity : CharacterComponent {

        [SerializeField]
        private float _gravity = 9.86f;

        public float Gravity {
            get { return _gravity; }
            set { _gravity = Mathf.Abs(value); }
        }

        private void FixedUpdate() {
            if (Character != null)
                Character.AddForce(-Vector3.up*_gravity);
        }

    }

}