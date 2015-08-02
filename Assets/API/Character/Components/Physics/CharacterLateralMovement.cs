using UnityEngine;
using System.Collections;
using Crescendo.API;

namespace Crescendo.API {
    
    public class CharacterLateralMovement : CharacterComponent
    {

        [SerializeField]
        private float _walkSpeed = 3f;

        [SerializeField]
        private float _dashSpeed = 5f;

        [SerializeField]
        private float _airSpeed = 3f;

        protected override void Start()
        {
            base.Start();

            if (Character == null)
                return;

            // Subscribe to Character events
            Character.OnMove += OnMove;
        }

        void OnDestroy()
        {
            if (Character == null)
                return;

            // Unsubscribe to Character events
            Character.OnMove -= OnMove;
        }

        void OnMove(Vector2 direction)
        {
            if (Mathf.Abs(direction.x) < float.Epsilon)
                return;

            Vector3 vel = Character.Velocity;

            if (Character.IsGrounded)
            {
                if (Character.IsDashing)
                    vel.x = _dashSpeed;
                else
                    vel.x = _walkSpeed;
            }
            else
                vel.x = _airSpeed;
            vel.x = Util.MatchSign(vel.x, direction.x);

            Character.Velocity = vel;

        }
    }
}