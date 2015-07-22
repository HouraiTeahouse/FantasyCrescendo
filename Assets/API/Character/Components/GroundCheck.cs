using UnityEngine;
using System.Collections;

namespace Genso.API {

    [RequireComponent(typeof(Collider))]
    public class GroundCheck : CharacterComponent {

        protected override void Awake() {
            base.Awake();
            var col = GetComponent<Collider>();
            col.enabled = true;
            col.isTrigger = true;
        }

        void OnTriggerEnter(Collider other) {
            SetGrounded(true, other);
        }

        void OnTriggerStay(Collider other)
        {
            SetGrounded(true, other);
        }

        void OnTriggerExit(Collider other) {
            SetGrounded(false, other);
        }

        void SetGrounded(bool value, Collider other) {
            if (Character == null || other == null)
                return;

            if (other.CompareTag("Platform")) {
                Character.IsGrounded = value;   
            }
        }
    }


}