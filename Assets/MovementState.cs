using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    [RequireComponent(typeof(PhysicsState))]
    public class MovementState : NetworkBehaviour {

        public enum CharacterFacingMode {
            Rotation, Scale
        }

        PhysicsState Physics { get; set; }

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

        [Header("Variables")]
        [SyncVar(hook = "OnChangeDirection"), SerializeField, ReadOnly]
        bool _direction;

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
        }

        public CharacterFacingMode FacingMode {
            get { return _characterFacingMode; }
        }

        void Awake() { Physics = GetComponent<PhysicsState>(); }

        void Start() {
            OnChangeDirection(_direction);
        }

        void Update() {
            if (!isLocalPlayer)
                return;
            float horizontalSpeed = 0;
            bool facing = Direction;

            if (Input.GetKey(KeyCode.A)) {
                horizontalSpeed = -RunSpeed;
                facing = false;
            } else if (Input.GetKey(KeyCode.LeftArrow)) {
                horizontalSpeed = -FastWalkSpeed;
                facing = false;
            }

            if (Input.GetKey(KeyCode.D)) {
                horizontalSpeed = RunSpeed;
                facing = true;
            } else if (Input.GetKey(KeyCode.RightArrow)) {
                horizontalSpeed = FastWalkSpeed;
                facing = true;
            }

            Physics.SetHorizontalVelocity(horizontalSpeed);
            if(Direction != facing)
                CmdSetDirection(facing);
        }

        [Command]
        void CmdSetDirection(bool direction) {
            _direction = direction;
        }

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

