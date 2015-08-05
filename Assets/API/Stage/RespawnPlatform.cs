using UnityEngine;

namespace Crescendo.API {

    public class RespawnPlatform : GensoBehaviour {

        [SerializeField]
        private float _invicibilityTimer;

        [SerializeField]
        private float _platformTimer;

        private Character character;
        private Rigidbody physics;
        private float timer;

        public Character Character {
            get { return character; }
            set {
                if (value == null)
                    return;
                value.IsInvincible = true;
                character = value;
                physics = value.GetComponent<Rigidbody>();
                if (physics != null)
                    physics.velocity = Vector3.zero;
            }
        }

        // Update is called once per frame
        private void Update() {
            if (character == null)
                return;

            timer += Util.dt;

            // TODO: Find better alternative to this hack
            if (timer > _platformTimer || (physics != null && physics.velocity.magnitude > 0.5f))
                Destroy(gameObject);
        }

        private void OnDestroy() {
            if (Character == null)
                return;

            Character.TemporaryInvincibility(_invicibilityTimer);
        }

    }

}