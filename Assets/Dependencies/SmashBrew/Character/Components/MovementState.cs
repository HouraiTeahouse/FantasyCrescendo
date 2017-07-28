using System;
using System.Collections.Generic;
using System.Linq;
using HouraiTeahouse.SmashBrew.Stage;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    [DisallowMultipleComponent]
    [AddComponentMenu("Smash Brew/Character/Movement State")]
    [RequireComponent(typeof(PhysicsState), typeof(InputState))]
    public class MovementState : CharacterNetworkComponent {

        public enum CharacterFacingMode {
            Rotation, Scale
        }

        PhysicsState PhysicsState { get; set; }
        CharacterController MovementCollider { get; set; }
        InputState InputState { get; set; }
        HitState HitState { get; set; }

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
        float[] _jumpPower = { 5f, 10f };

        [SerializeField]
        Transform _ledgeTarget;


        [Header("Variables")]
        [SyncVar(hook = "OnChangeDirection")]
        bool _direction;

        [SyncVar]
        int _jumpCount;

        [SyncVar]
        bool _isFastFalling;

        [SerializeField]
        Transform _skeletonRoot;

        public float MaxFallSpeed {
            get { return _maxFallSpeed; }
        }

        public float FastFallSpeed {
            get { return _fastFallSpeed; }
        }

        public bool Direction {
            get { return _direction; }
            set {
                if(_direction != value && hasAuthority)
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


        /// <summary> Can the Character currently jump? </summary>
        public bool CanJump {
            get { return JumpCount < MaxJumpCount; }
        }

        public Transform LedgeTarget {
            get { return _ledgeTarget; }
        }

        Transform _currentLedge;
        public Transform CurrentLedge {
            get { return _currentLedge; }
            set {
                bool grabbed = _currentLedge == null && value != null;
                _currentLedge = value;
                if (grabbed)
                    SnapToLedge();
            }
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            InputState = this.SafeGetComponent<InputState>();
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() {
            PhysicsState = this.SafeGetComponent<PhysicsState>();
            MovementCollider = this.SafeGetComponent<CharacterController>();
            HitState = GetComponent<HitState>();
            JumpCount = MaxJumpCount;
            OnChangeDirection(_direction);
            _ledgeTarget = this.CachedGetComponent(_ledgeTarget, () => transform);
            if (Character == null)
                return;
            var stateController = Character.StateController;
            var states = Character.States;
            stateController.OnStateChange += (b, a) => {
                if (states.Jump == a)
                    Jump();
                if (states.LedgeRelease == a)  {
                    IsFastFalling = false;
                    CurrentLedge = null;
                }
                if (states.LedgeAttack == b || states.LedgeClimb == b) {
                    CurrentLedge = null;
                    transform.position = _skeletonRoot.position;
                    _skeletonRoot.position = transform.position;
                    MovementCollider.Move(Vector3.down * MovementCollider.height);
                    Character.StateController.SetState(Character.States.Idle);
                }
                if (states.EscapeForward == b && isServer)
                    _direction = !_direction;
            };
        }

        struct MovementInfo {
            public Vector2 Speed;
            public bool facing;
        }

        void SnapToLedge() {
            IsFastFalling = false;
            var offset = LedgeTarget.position - transform.position;
            transform.position = CurrentLedge.position - offset;
            if (JumpCount != MaxJumpCount)
                CmdResetJumps();
        }

        void LedgeMovement() {
            if (JumpCount != MaxJumpCount)
                CmdResetJumps();
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || InputState.Smash.y <= -DirectionalInput.DeadZone) {
                CurrentLedge = null;
            } else {
                SnapToLedge();
            }
        }

        void Jump() {
            CurrentLedge = null;
            OnJump.SafeInvoke();
            PhysicsState.SetVerticalVelocity(_jumpPower[MaxJumpCount - JumpCount]);
            CmdJump();
        }

        bool GetKeys(params KeyCode[] keys) {
            return keys.Any(Input.GetKey);
        }

        bool GetKeysDown(params KeyCode[] keys) {
            return keys.Any(Input.GetKeyDown);
        }

        float ButtonAxis(bool neg, bool pos) {
            var val = neg ? -1f : 0f;
            return val + (pos ? 1f : 0f);
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() {
            if (!isLocalPlayer)
                return;
            if (Mathf.Approximately(Time.deltaTime, 0))
                return;
            if (HitState != null && HitState.Hitstun > 0)
                return;
            if (CurrentState.Data.MovementType == MovementType.Locked) {
                PhysicsState.SetHorizontalVelocity(0f);
                PhysicsState.SetVerticalVelocity(0f);
                return;
            }
            var movement = new MovementInfo { facing = Direction };
            // If currently hanging from a edge
            if (CurrentLedge != null) {
                LedgeMovement();
            } else {
                var movementInput = InputState.Movement;
                if (PhysicsState.IsGrounded) {
                    IsFastFalling = false;
                    if (JumpCount != MaxJumpCount)
                        CmdResetJumps();
                    if (movementInput.x > DirectionalInput.DeadZone)
                        movement.facing = true;
                    if (movementInput.x < -DirectionalInput.DeadZone)
                        movement.facing = false;
                    Direction = movement.facing;
                } else {
                    if (GetKeysDown(KeyCode.S, KeyCode.DownArrow) || InputState.Smash.y < -DirectionalInput.DeadZone)
                        IsFastFalling = true;
                    LimitFallSpeed();
                }
                movement = ApplyControlledMovement(movement, movementInput);
            }
            PhysicsState.SetHorizontalVelocity(movement.Speed.x);
        }

        MovementInfo ApplyControlledMovement(MovementInfo movement, Vector2 movementInput) {
            switch(CurrentState.Data.MovementType) {
                case MovementType.Normal:
                    var dir = 1f;
                    dir = Direction ? 1f : -1f;
                    movement.Speed.x =  dir * Mathf.Abs(movementInput.x) * CurrentState.Data.MovementSpeed.Max;
                    break;
                case MovementType.DirectionalInfluenceOnly:
                    movement.Speed.x = movementInput.x * CurrentState.Data.MovementSpeed.Max;
                    break;
            }
            return movement;
        }

        void LimitFallSpeed() {
            var yVel = PhysicsState.Velocity.y;
            if (IsFastFalling)
                PhysicsState.SetVerticalVelocity(-FastFallSpeed);
            else if (yVel < -MaxFallSpeed)
                PhysicsState.SetVerticalVelocity(-MaxFallSpeed);
        }

        void OnControllerColliderHit(ControllerColliderHit hit) {
            var platform = hit.gameObject.GetComponent<Platform>();
            if (platform != null)
                platform.CharacterCollision(MovementCollider);
        }

        public override void UpdateStateContext(CharacterStateContext context) {
            context.IsGrabbingLedge = CurrentLedge != null;
            context.Direction = Direction ? 1.0f : -1.0f;
            context.CanJump = JumpCount > 0 && JumpCount <= MaxJumpCount;
        }

        [Command]
        void CmdJump() { JumpCount--; }

        [Command]
        void CmdResetJumps() { JumpCount = MaxJumpCount; }

        [Command]
        void CmdSetDirection(bool direction) {
            if (CurrentState.Data.CanTurn)
                _direction = direction;
        }

        [ClientRpc]
        public void RpcMove(Vector2 position) { transform.position = position; }

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
