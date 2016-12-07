using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    [RequireComponent(typeof(CharacterController))]
    public class PhysicsState : NetworkBehaviour, ICharacterState {

        // Character Constrants
        [Header("Constants")]
        [SerializeField]
        [Tooltip("How much the character weighs")]
        float _weight = 1.0f;

        [SerializeField]
        [Tooltip("How fast a charactter reaches their max fall speed, in seconds.")]
        float _gravity = 1.5f;

        // Character Variables 
        [Header("Variables")]
        [SerializeField, ReadOnly]
        Vector2 _velocity;

        [SerializeField, ReadOnly]
        Vector2 _acceleration;

        [SyncVar, SerializeField, ReadOnly]
        bool _grounded;

        [SyncVar, SerializeField, ReadOnly]
        bool _isFastFalling;

        public float Weight {
            get { return _weight; }
        }

        public float Gravity {
            get { return _gravity; }
        }

        public Vector2 Velocity {
            get { return _velocity; }
            set { _velocity = value; }
        }

        public Vector2 Acceleration {
            get { return _acceleration; }
            set { _acceleration = value; }
        }

        public void ResetState() {
            Velocity = Vector2.zero;
            Acceleration = Vector2.zero;
        }

        public CharacterController CharacterController { get; private set; }

        public bool IsFastFalling {
            get { return _isFastFalling; }
        }

        public void SetHorizontalVelocity(float speed) { _velocity.x = speed; }
        public void SetVerticalVelocity(float speed) { _velocity.y = speed; }

        void Awake() { CharacterController = GetComponent<CharacterController>(); }

        void Update() {
            if (!isLocalPlayer)
                return;
            var acceleration = Acceleration + Vector2.down * Gravity;
            Log.Debug(acceleration * Time.deltaTime + " " + Time.deltaTime);
            if (CharacterController.isGrounded)
                acceleration.y = 0;
            Velocity += acceleration * Time.deltaTime;
            CharacterController.Move(Velocity * Time.deltaTime);
        }

    }

}

