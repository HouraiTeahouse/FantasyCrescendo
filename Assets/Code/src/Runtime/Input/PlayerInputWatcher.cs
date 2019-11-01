using HouraiTeahouse.FantasyCrescendo.Matches;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using PlayerInputComponent = UnityEngine.InputSystem.PlayerInput;

namespace HouraiTeahouse.FantasyCrescendo {

[RequireComponent(typeof(PlayerInputComponent))]
public class PlayerInputWatcher : MonoBehaviour {

    class TapInputDetector {

        static InputConfig InputConfig;

        public Vector2 LastRawInput;
        public Vector2 CurrentRawInput;

        public Vector2 SmashValue;
        public int SmashFramesRemaining;

        public TapInputDetector() {
            if (InputConfig == null) {
                InputConfig = Config.Get<InputConfig>();
            }
        }

        public void Update(Vector2 newInput) {
            LastRawInput = CurrentRawInput;
            CurrentRawInput = newInput;

            SmashFramesRemaining = Mathf.Max(0, SmashFramesRemaining - 1);

            var lastInput = InputUtil.EnforceDeadZone(LastRawInput);
            var currentInput = InputUtil.EnforceDeadZone(CurrentRawInput);
            if (InputUtil.OutsideDeadZone(lastInput)) {
                SmashValue = Vector2.zero;
                return;
            } 

            var diff = currentInput - lastInput;
            diff = InputUtil.EnforceDeadZone(diff, InputConfig.SmashThreshold);
            diff = InputUtil.MaxComponent(diff);

            if (SmashFramesRemaining > 0) {
                // Has recently smashed, needs to be in a different direction to change
                var currentDirection = DirectionalInput.GetDirection(SmashValue);
                var newDirection = DirectionalInput.GetDirection(diff);
                if (currentDirection != newDirection) {
                RefreshSmashValue(diff);
                }
            } else if (!InputUtil.OutsideDeadZone(diff, InputConfig.SmashThreshold)) {
                SmashValue = Vector2.zero;
            } else {
                RefreshSmashValue(diff);
            }
        }

        void RefreshSmashValue(Vector2 value) {
            SmashValue = value;
            SmashFramesRemaining = (int)InputConfig.SmashFrameWindow;
        }

    }

    PlayerInputComponent _inputSource;

    [SerializeField] string _movementAction = "Match/Move";
    [SerializeField] string _strongAction   = "Match/Strong";
    [SerializeField] string _attackAction   = "Match/Attack";
    [SerializeField] string _specialAction  = "Match/Special";
    [SerializeField] string _jumpAction     = "Match/Jump";
    [SerializeField] string _grabAction     = "Match/Grab";
    [SerializeField] string _shieldAction   = "Match/Shield";

    void Start() {
        _inputSource = GetComponent<PlayerInputComponent>();
        Assert.IsNotNull(_inputSource);
        var index = _inputSource.playerIndex;
        FindObjectOfType<InputManager>().ForceNull()?.Register(index, this);
        Debug.Log($"Local player {index} started.");
        gameObject.name = gameObject.name.Replace("(Clone)", "") + " " + index.ToString();
    }

    public PlayerInput GetLatestInputs() {
        return new PlayerInput {
            Movement = (FixedVector16)GetAction(_movementAction).ReadValue<Vector2>(),
            Smash    = (FixedVector16)GetAction(_strongAction).ReadValue<Vector2>(),
            Attack   = GetAction(_attackAction).ReadValue<bool>(),
            Special  = GetAction(_specialAction).ReadValue<bool>(),
            Jump     = GetAction(_jumpAction).ReadValue<bool>(),
            Grab     = GetAction(_grabAction).ReadValue<bool>(),
            Shield   = GetAction(_shieldAction).ReadValue<bool>(),
        };
    }

    InputAction GetAction(string key) {
        Assert.IsNotNull(_inputSource);
        return _inputSource.actions[key];
    }

    public void OnPause(InputAction.CallbackContext value) {
        Debug.Log("OnPause");
        var matchManager = MatchManager.Instance;
        if (matchManager == null) return;
        matchManager.TogglePaused(_inputSource.playerIndex);
    }

    public void OnReset(InputAction.CallbackContext value) {
        Debug.Log("OnReset");
        var matchManager = MatchManager.Instance;
        if (matchManager == null) return;
        matchManager.ResetMatch(_inputSource.playerIndex);
    }

}

}
