using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using MatchPlayerInput = HouraiTeahouse.FantasyCrescendo.PlayerInput;

namespace HouraiTeahouse.FantasyCrescendo {

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputWatcher : MonoBehaviour {

  PlayerInput _inputSource;

  [SerializeField] string _movementAction;
  [SerializeField] string _strongAction;
  [SerializeField] string _attackAction;
  [SerializeField] string _specialAction;
  [SerializeField] string _jumpAction;
  [SerializeField] string _grabAction;
  [SerializeField] string _shieldAction;

  void Awake() {
    _inputSource = GetComponent<PlayerInput>();
    Assert.IsNotNull(_inputSource);
  }

  public MatchPlayerInput GetLatestInputs() {
    return MatchPlayerInput {
      Movement = (FixedVector2)GetAction(_movementAction).ReadValue<Vector2>(),
      Strong   = (FixedVector2)GetAction(_strongAction).ReadValue<Vector2>(),
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

  void OnPause(InputValue value) {
    // TODO(james7132): Implement
  }

  void OnReset(InputValue value) {
    // TODO(james7132): Implement
  }

}

}
