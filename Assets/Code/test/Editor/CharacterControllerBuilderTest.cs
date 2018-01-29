using HouraiTeahouse.FantasyCrescendo.Characters;
using HouraiTeahouse.FantasyCrescendo.Players;
using HouraiTeahouse.FantasyCrescendo.Matches;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Parallelizable]
public class CharacterControllerBuilderTest {

  static StateController<CharacterState, CharacterContext> _stateController;
  static Dictionary<string, CharacterState> _stateMap;

  [SetUp]
  public void Setup() {
      if (_stateMap != null && _stateController != null)
          return;
      var instance = ScriptableObject.CreateInstance<CharacterControllerBuilder>();
      var builder = new StateControllerBuilder<CharacterState, CharacterContext>();
      _stateController = instance.BuildCharacterControllerImpl(builder);
      _stateMap = _stateController.States.ToDictionary(s => s.Name, s => s);
      UnityEngine.Object.DestroyImmediate(instance);
  }

  static PlayerInputContext CreateInput(PlayerInput input) => new PlayerInputContext { Current = input };

  static IEnumerable<object[]> LedgeTestCases() {
    var ledgeState = new PlayerState { GrabbedLedgeID = 20 };
    yield return new object[] { "Idle", "LedgeGrab", new CharacterContext { 
      State = ledgeState,
      IsGrounded = true
    }};
    foreach (var src in new[] {"Fall", "FallHelpless"}) {
      yield return new object[] { src, "LedgeGrab", new CharacterContext { 
        State = ledgeState,
        IsGrounded = false
      }};
    }
    yield return new object[] {"LedgeGrab", "LedgeIdle", new CharacterContext { 
      State = new PlayerState { NormalizedStateTime = 1.0f }
    }};
    yield return new object[] {"LedgeIdle", "LedgeRelease", new CharacterContext {
      Input = CreateInput(new PlayerInput { Movement = new Vector2(0, -1) })
    }};
    yield return new object[] {"LedgeIdle", "LedgeAttack", new CharacterContext {
      State = ledgeState,
      Input = CreateInput(new PlayerInput { Attack =  true })
    }};
    yield return new object[] {"LedgeIdle", "LedgeJump", new CharacterContext {
      State = ledgeState,
      CanJump = true,
      Input = CreateInput(new PlayerInput { Jump =  true })
    }};
  }

  static IEnumerable<object[]> MovementTestCases() {
    Func<float, float, bool, bool, CharacterContext>
      context = (x, y, d, g) => new CharacterContext {
        State = new PlayerState { Direction = d },
        IsGrounded = g,
        Input = CreateInput(new PlayerInput { Movement = new Vector2(x, y) })
      };
    foreach (var dir in new[] {true, false}) {
      foreach (var src in new[] {"Idle", "Walk"}) {
        yield return new object[] {src, "Idle", context(0f, 0f, dir, true)};
        yield return new object[] {src, "Walk", context(1f, 0f, dir, true)};
      }
      foreach (var src in new[] {"Idle", "Walk", "CrouchStart", "Crouch", "CrouchEnd"}) {
        yield return new object[] {src, "Fall", context(0f, 0f, dir, false)};
      }
      foreach (var src in new[] {"FallHelpless", "Fall", "EscapeAir"}) {
        yield return new object[] {src, "Land", context(0f, 0f, dir, true)};
      }
      var direction = dir ? 1.0f : -1.0f;
      var directionState = new PlayerState { Direction = dir };
      yield return new object[] {"Idle", "Dash", new CharacterContext {
        State = directionState,
        IsGrounded = true,
        Input = CreateInput(new PlayerInput { Smash = -Vector2.right })
      }};
      yield return new object[] {"Run", "RunTurn", new CharacterContext {
        State = directionState,
        IsGrounded = true,
        Input = CreateInput(new PlayerInput { Movement = Vector2.right * -direction })
      }};
      yield return new object[] {"Run", "Run", new CharacterContext {
        State = directionState,
        IsGrounded = true,
        Input = CreateInput(new PlayerInput { Movement = Vector2.right * direction })
      }};
      foreach (var src in new [] {"Idle", "Walk", "Dash", "Run", "RunTurn", "RunBrake", "CrouchStart", "Crouch", "CrouchEnd", "Shield.Main"}) {
        yield return new object[] {src, "JumpStart", new CharacterContext {
          State = new PlayerState { Direction = dir },
          IsGrounded = true,
          CanJump = true,
          Input = CreateInput(new PlayerInput { Jump = true })
        }};
      }
    }
  }

  static IEnumerable<object[]> AttackTestCases() {
    Func<float, float, bool, bool, CharacterContext>
      context = (x, y, d, g) => new CharacterContext {
        State = new PlayerState { Direction = d },
        Input = CreateInput(new PlayerInput {
          Movement = new Vector2(x, y),
          Attack = true
        }) 
      };
    const float magnitude = 1.0f;
    foreach (var src in new[] {"Idle", "Walk", "CrouchStart", "Crouch", "CrouchEnd"}) {
      foreach (var dir in new[] {true, false}) {
        yield return new object[] {src, "Neutral", context(0f, 0f, dir, true)};
        yield return new object[] {src, "TiltUp", context(0f, magnitude, dir, true)};
        yield return new object[] {src, "TiltDown", context(0f, -magnitude, dir, true)};
        yield return new object[] {src, "TiltSide", context(magnitude, 0f, dir, true)};
        yield return new object[] {src, "TiltSide", context(-magnitude, 0f, dir, true)};
      }
    }
    foreach (var src in new[] {"Fall", "Jump", "JumpAerial"}) {
      foreach (var dir in new[] {true, false}) {
        yield return new object[] {src, "AerialNeutral", context(0f, 0f, dir, false)};
        yield return new object[] {src, "AerialUp", context(0f, magnitude, dir, false)};
        yield return new object[] {src, "AerialDown", context(0f, -magnitude, dir, false)};
      }
      yield return new object[] {src, "AerialForward", context(magnitude, 0f, true, false)};
      yield return new object[] {src, "AerialBackward", context(-magnitude, 0f, true, false)};
      yield return new object[] {src, "AerialBackward", context(magnitude, 0f, false, false)};
      yield return new object[] {src, "AerialForward", context(-magnitude, 0f, false, false)};
    }
    // TODO(james7132): Dash Attacks
    // foreach (var src in new[] {"Dash", "Run", "RunTurn", "RunBrake"}) {
    //     foreach (var dir in new[] {true, false}) {
    //         yield return new object[] {src, "", context(magnitude, 0f, true, false)};
    //     }
    // }
  }

  static IEnumerable<object[]> AutomaticTestCases() {
    var cases = new Dictionary<string, string>{
      {"Shield.On", "Shield.Main"},
      {"Shield.Broken", "Shield.Stunned"},
      {"Shield.Stunned", "Idle"},
      {"Land", "Idle"},
      {"CrouchEnd", "Idle"},
      {"RunBrake", "Idle"},
      {"AerialAttackLand", "Idle"},
      {"Neutral", "Idle"},
      {"TiltUp", "Idle"},
      {"TiltDown", "Idle"},
      {"TiltSide", "Idle"},
      {"SmashUp.Charge", "SmashUp.Attack"},
      {"SmashSide.Charge", "SmashSide.Attack"},
      {"SmashDown.Charge", "SmashDown.Attack"},
      {"SmashUp.Attack", "Idle"},
      {"SmashSide.Attack", "Idle"},
      {"SmashDown.Attack", "Idle"},
      {"EscapeAir", "FallHelpless"},
      {"Escape", "Shield.Main"},
      {"EscapeForward", "Shield.Main"},
      {"EscapeBackward", "Shield.Main"},
      {"Dash", "Idle"},
      {"RunTurn", "Run"},
      {"LedgeGrab", "LedgeIdle"},
      {"LedgeJump", "Jump"},
      {"JumpStart", "Jump"},
      {"JumpAerial", "Jump"},
      {"CrouchStart", "Crouch"},
    };
    foreach(var kvp in cases) {
      yield return new object[] {kvp.Key, kvp.Value, new CharacterContext {
        State = new PlayerState { NormalizedStateTime = 1.0f },
        CanJump = true,
        Input = CreateInput(new PlayerInput())
      }};
    }
    yield return new object[] {"TiltDown", "Crouch", new CharacterContext {
      State = new PlayerState { NormalizedStateTime = 1.0f },
      Input = CreateInput(new PlayerInput { Movement = -Vector2.up })
    }};
    yield return new object[] {"Dash", "Run", new CharacterContext {
      State = new PlayerState { NormalizedStateTime = 1.0f },
      Input = CreateInput(new PlayerInput { Movement = Vector2.right})
    }};
  }

  void TestTransition(string src, string dst, CharacterContext context) {
    if (context.Input == null) context.Input = new PlayerInputContext();
    _stateController.SetState(_stateMap[src]);
    _stateController.UpdateState(context);
    Assert.AreEqual(dst, _stateController.CurrentState.Name);
  }

  [Test, TestCaseSource("AutomaticTestCases")]
  public void automatic_exits(string src, string dst, CharacterContext context = null) {
    TestTransition(src, dst, context ?? new CharacterContext {
      State = new PlayerState {
        NormalizedStateTime = 1.0f,
        ShieldDamage = 100
      }
    });
  }        

  [Test, TestCaseSource("AttackTestCases")]
  public void attack_transitions(string src, string dst, CharacterContext context) {
    TestTransition(src, dst, context);
  }

  [Test, TestCaseSource("MovementTestCases")]
  public void movement_transitions(string src, string dst, CharacterContext context) {
    TestTransition(src, dst, context);
  }

  [Test, TestCaseSource("LedgeTestCases")]
  public void ledge_transitions(string src, string dst, CharacterContext context) {
    TestTransition(src, dst, context);
  }

}