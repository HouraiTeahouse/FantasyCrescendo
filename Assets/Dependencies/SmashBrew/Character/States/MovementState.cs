using System;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    [DisallowMultipleComponent]
    [AddComponentMenu("Smash Brew/Character/Movement State")]
    [RequireComponent(typeof(PhysicsState))]
    public class MovementState : NetworkBehaviour {

        public enum CharacterFacingMode {
            Rotation, Scale
        }

        PhysicsState Physics { get; set; }
        CharacterController CharacterController { get; set; }

        public event Action OnJump;

        [Header("Constants")]
        [SerializeField]
        CharacterFacingMode _characterFacingMode;

        [SerializeField]
        [Range(2f, 10f)]
        [Tooltip("The maximum downward velocity of the character under normal conditions")]
        float _maxFallSpeed = 5f;

        [SerializeField]
        [Range(2f, 10f)]
        [Tooltip("The downward velocity of the character while fast falling")]
        float _fastFallSpeed = 5f;

        [SerializeField]
        [Range(2f, 10f)]
        [Tooltip("The minimum walking speed of the character")]
        float _slowWalkSpeed = 2f;

        [SerializeField]
        [Range(2f, 10f)]
        [Tooltip("The maximum walking speed of the character")]
        float _fastWalkSpeed = 4f;

        [SerializeField]
        [Range(2f, 10f)]
        [Tooltip("The running speed of the character")]
        float _runSpeed = 6f;

        [SerializeField]
        [Range(2f, 10f)]
        [Tooltip("The horizontal speed of the character while in the air")]
        float _airSpeed = 4f;

        [SerializeField]
        float[] _jumpPower = { 5f, 10f };

        [SerializeField]
        Transform _ledgeTarget;

        [Header("Variables")]
        [SerializeField, ReadOnly]
        Transform _currentLedge;

        [SyncVar(hook = "OnChangeDirection"), SerializeField, ReadOnly]
        bool _direction;

        [SyncVar, SerializeField, ReadOnly]
        int _jumpCount;

        [SyncVar, SerializeField, ReadOnly]
        bool _isFastFalling;

        public float MaxFallSpeed {
            get { return _maxFallSpeed; }
        }

        public float FastFallSpeed {
            get { return _fastFallSpeed; }
        }

        public float SlowWalkSpeed {
            get { return _slowWalkSpeed; }
        }

        public float FastWalkSpeed {
            get { return _fastWalkSpeed; }
        }

        public float RunSpeed {
            get { return _runSpeed; }
        }

        public bool Direction {
            get { return _direction; }
            set {
                if(_direction != value)
                    CmdSetDirection(value);
            }
        }

        public CharacterFacingMode FacingMode {
            get { return _characterFacingMode; }
        }

        public int JumpCount {
            get { return _jumpCount; }
            private set { _jumpCount = value; }
        }

        public int MaxJumpCount {
            get { return _jumpPower != null ? _jumpPower.Length : 0; }
        }

        public bool IsFastFalling {
            get { return _isFastFalling; }
            private set { _isFastFalling = value; }
        }

        public bool IsCrounching {
            get { return CharacterController.isGrounded && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)); }
        }

        /// <summary> Can the Character currently jump? </summary>
        public bool CanJump {
            get { return JumpCount < MaxJumpCount; }
        }

        public float AirSpeed {
            get { return _airSpeed; }
        }

        public Transform LedgeTarget {
            get { return _ledgeTarget; }
        }

        public Transform CurrentLedge {
            get { return _currentLedge; }
            set {
                _currentLedge = value;
                CmdResetJumps();
            }
        }

        void Start() {
            Physics = this.SafeGetComponent<PhysicsState>();
            CharacterController = this.SafeGetComponent<CharacterController>();
            JumpCount = MaxJumpCount;
            OnChangeDirection(_direction);
            if (_ledgeTarget == null)
                _ledgeTarget = transform;
        }

        struct MovementInfo {
            public float horizontalSpeed;
            public bool facing;
        }

        void Update() {
            if (!isLocalPlayer)
                return;

            var movement = new MovementInfo { facing = Direction };
            // If currently hanging from a edge
            if (CurrentLedge != null) {
                IsFastFalling = false;
                var offset = LedgeTarget.position - transform.position;
                transform.position = CurrentLedge.position - offset;
                if (JumpCount != MaxJumpCount)
                    CmdResetJumps();
            } else {
                if(CharacterController.isGrounded)
                    movement = GroundedMovement(movement);
                else
                    movement = AerialMovement(movement);
            }

            if (!IsCrounching && JumpCount > 0 && JumpCount <= MaxJumpCount &&
                (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))) {
                Physics.SetVerticalVelocity(_jumpPower[MaxJumpCount - JumpCount]);
                CurrentLedge = null;
                OnJump.SafeInvoke();
                CmdJump();
            }

            LimitFallSpeed();

            Physics.SetHorizontalVelocity(movement.horizontalSpeed);
            if (Direction != movement.facing)
                CmdSetDirection(movement.facing);
        }

        MovementInfo GroundedMovement(MovementInfo info) {
            IsFastFalling = false;

            if (Input.GetKey(KeyCode.A)) {
                info.horizontalSpeed = -RunSpeed;
                info.facing = false;
            } else if (Input.GetKey(KeyCode.LeftArrow)) {
                info.horizontalSpeed = -FastWalkSpeed;
                info.facing = false;
            }

            if (Input.GetKey(KeyCode.D)) {
                info.horizontalSpeed = RunSpeed;
                info.facing = true;
            } else if (Input.GetKey(KeyCode.RightArrow)) {
                info.horizontalSpeed = FastWalkSpeed;
                info.facing = true;
            }

            if (JumpCount != MaxJumpCount)
                CmdResetJumps();
            if (IsCrounching)
                info.horizontalSpeed = 0f;
            return info;
        }

        MovementInfo AerialMovement(MovementInfo info) {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
                info.horizontalSpeed = -AirSpeed;
                info.facing = false;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
                info.horizontalSpeed = FastWalkSpeed;
                info.facing = true;
            }

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                IsFastFalling = true;

            return info;
        }

        void LimitFallSpeed() {
            var yVel = Physics.Velocity.y;
            if (IsFastFalling)
                Physics.SetVerticalVelocity(-FastFallSpeed);
            else if (yVel < -MaxFallSpeed)
                Physics.SetVerticalVelocity(-MaxFallSpeed);
        }

        [Command]
        void CmdJump() { JumpCount--; }

        [Command]
        void CmdResetJumps() { JumpCount = MaxJumpCount; }

        [Command]
        void CmdSetDirection(bool direction) { _direction = direction; }

        void OnChangeDirection(bool direction) {
            _direction = direction;
            if (FacingMode == CharacterFacingMode.Rotation) {
                var euler = transform.localEulerAngles;
                euler.y = direction ? 0 : 180;
                transform.localEulerAngles = euler;
            } else {
                var scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * (direction ? 1 : -1);
                transform.localScale = scale;
            }
        }

    }

}
