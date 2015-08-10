using UnityEngine;

namespace Crescendo.API {

    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    public class CharacterMovement : RestrictableCharacterComponent {

        [SerializeField]
        private float _airSpeed = 3f;

        [SerializeField]
        private float _dashSpeed = 5f;

        [SerializeField]
        private float _walkSpeed = 3f;

        protected override void Start() {
            base.Start();

            if (Character == null)
                return;

            // Subscribe to Character events
            Character.OnMove += OnMove;
        }

        private void OnDestroy() {
            if (Character == null)
                return;

            // Unsubscribe to Character events
            Character.OnMove -= OnMove;
        }

        private void OnMove(Vector2 direction) {
            if (Mathf.Abs(direction.x) < float.Epsilon)
                return;

            Vector3 vel = Character.Velocity;

            if (Character.IsIsGrounded) {
                if (Character.IsDashing)
                    vel.x = _dashSpeed;
                else
                    vel.x = _walkSpeed;
            } else
                vel.x = _airSpeed;
            vel.x = Util.MatchSign(vel.x, direction.x);

            Character.Velocity = vel;
        }

    }

}