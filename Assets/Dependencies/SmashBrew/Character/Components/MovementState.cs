using System;
using System.Linq;
using HouraiTeahouse.SmashBrew.Stage;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    [DisallowMultipleComponent]
    [AddComponentMenu("Smash Brew/Character/Movement State")]
    [RequireComponent(typeof(PhysicsState))]
    public class MovementState : CharacterComponent {

        public enum CharacterFacingMode {
            Rotation, Scale
        }

        PhysicsState PhysicsState { get; set; }
        CharacterController CharacterController { get; set; }
        InputContext _input;

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
        [SerializeField, ReadOnly]
        Transform _currentLedge;

        [SyncVar(hook = "OnChangeDirection"), ReadOnly]
        bool _direction;

        [SyncVar, ReadOnly]
        int _jumpCount;

        [SyncVar, ReadOnly]
        bool _isFastFalling;

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

        [SerializeField]
        LayerMask _stageLayers = -1;

        public bool IsGrounded {
            get { 
                if (PhysicsState != null && PhysicsState.Velocity.y > 0)
                    return false;
                var center = Vector3.zero;
                var radius = 1f;
                if (CharacterController != null) {
                    center = CharacterController.center - Vector3.up * (CharacterController.height * 0.50f - CharacterController.radius * 0.5f);
                    radius = CharacterController.radius * 0.75f;
                }
                return Physics.OverlapSphere(transform.TransformPoint(center), radius, _stageLayers).Any();
            }
        }

        /// <summary>
        /// Callback to draw gizmos that are pickable and always drawn.
        /// </summary>
        void OnDrawGizmos() {
            var center = Vector3.zero;
            var radius = 1f;
            if (CharacterController != null) {
                center = CharacterController.center - Vector3.up * (CharacterController.height * 0.5f - CharacterController.radius * 0.5f);
                radius = CharacterController.radius * 0.75f;
                var diff = Vector3.up * (CharacterController.height * 0.5f - CharacterController.radius);
                using (Gizmo.With(Color.red)) {
                    var rad =  CharacterController.radius * transform.lossyScale.Max();
                    Gizmos.DrawWireSphere(transform.TransformPoint(CharacterController.center + diff), rad);
                    Gizmos.DrawWireSphere(transform.TransformPoint(CharacterController.center - diff), rad);
                }
            }
            using (Gizmo.With(Color.blue)) {
                Gizmos.DrawWireSphere(transform.TransformPoint(center), radius);
            }
        }

        /// <summary> Can the Character currently jump? </summary>
        public bool CanJump {
            get { return JumpCount < MaxJumpCount; }
        }

        public Transform LedgeTarget {
            get { return _ledgeTarget; }
        }

        public Transform CurrentLedge {
            get { return _currentLedge; }
            set {
                bool grabbed = _currentLedge == null && value != null;
                _currentLedge = value;
                if (grabbed)
                    SnapToLedge();
            }
        }

        void Start() {
            PhysicsState = this.SafeGetComponent<PhysicsState>();
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

        void SnapToLedge() {
            IsFastFalling = false;
            var offset = LedgeTarget.position - transform.position;
            transform.position = CurrentLedge.position - offset;
            if (JumpCount != MaxJumpCount)
                CmdResetJumps();
        }

        void LedgeMovement() {
            if (JumpCheck()) {
            } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
                CurrentLedge = null;
            } else {
                SnapToLedge();
            }
        }

        bool JumpCheck() {
            bool success = (JumpCount > 0 && JumpCount <= MaxJumpCount
                && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)));
            if (success) {
                CurrentLedge = null;
                OnJump.SafeInvoke();
                PhysicsState.SetVerticalVelocity(_jumpPower[MaxJumpCount - JumpCount]);
                CmdJump();
            }
            return success;
        }

        void UpdateInput() {
            // TODO(james7132): Hook this up to the full input system
            var movement = Vector2.zero;
            var smash = Vector2.zero;
            if (Input.GetKey(KeyCode.A))
                smash.x += -1;
            if (Input.GetKey(KeyCode.D)) 
                smash.x += 1;
            if (Input.GetKey(KeyCode.W)) 
                smash.y += 1;
            if (Input.GetKey(KeyCode.S)) 
                smash.y += -1;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                movement.x += -1;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) 
                movement.x += 1;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) 
                movement.y += 1;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) 
                movement.y += -1;
            _input.Movement = movement;
            _input.Smash = smash;
        }

        void Update() {
            UpdateInput();
            if (!isLocalPlayer)
                return;
            if (Mathf.Approximately(Time.deltaTime, 0))
                return;

            var movement = new MovementInfo { facing = Direction };
            // If currently hanging from a edge
            if (CurrentLedge != null) {
                LedgeMovement();
            } else {
                movement.horizontalSpeed = _input.Movement.x * CurrentState.Data.MovementSpeed.Max;
                if (!Mathf.Approximately(_input.Movement.x, 0f))
                    movement.facing = _input.Movement.x > 0;
                if (IsGrounded) {
                    IsFastFalling = false;
                    if (JumpCount != MaxJumpCount)
                        CmdResetJumps();
                } else {
                    if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                        IsFastFalling = true;
                    LimitFallSpeed();
                }
                JumpCheck();
            }

            PhysicsState.SetHorizontalVelocity(movement.horizontalSpeed);
            Direction = movement.facing;
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
                platform.CharacterCollision(CharacterController);
        }

        public override void UpdateStateContext(CharacterStateContext context) {
            context.IsGrounded = IsGrounded;
            context.IsGrabbingLedge = CurrentLedge != null;
            context.Direction = Direction ? 1.0f : -1.0f;
            context.Input = _input;
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
