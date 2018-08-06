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

  [OneTimeSetUp]
  public void OneTimeSetup() {
    var instance = ScriptableObject.CreateInstance<CharacterControllerBuilder>();
    var builder = new StateControllerBuilder<CharacterState, CharacterContext>();
    _stateController = instance.BuildCharacterControllerImpl(builder);
    _stateMap = _stateController.States.ToDictionary(s => s.Name, s => s);
    UnityEngine.Object.DestroyImmediate(instance);
  }

  static PlayerInputContext CreateInput(PlayerInput input) => new PlayerInputContext { Current = input };

  static IEnumerable<TestCaseData> LedgeTestCases() {
    var ledgeState = new PlayerState { GrabbedLedgeID = 20 };
    yield return TestCase("Idle", "LedgeGrab", new CharacterContext { 
      State = ledgeState,
      IsGrounded = true
    });
    foreach (var src in new[] {"Fall", "FallHelpless"}) {
      yield return TestCase(src, "LedgeGrab", new CharacterContext { 
        State = ledgeState,
        IsGrounded = false
      });
    }
    yield return TestCase("LedgeGrab", "LedgeIdle", new CharacterContext { 
      State = new PlayerState { StateTick = 101 },
      StateLength = 100 * Time.fixedDeltaTime
    });
    yield return TestCase("LedgeIdle", "LedgeRelease", new CharacterContext {
      Input = CreateInput(new PlayerInput { Movement = new Vector2(0, -1) })
    });
    yield return TestCase("LedgeIdle", "LedgeAttack", new CharacterContext {
      State = ledgeState,
      Input = CreateInput(new PlayerInput { Attack =  true })
    });
    yield return TestCase("LedgeIdle", "LedgeJump", new CharacterContext {
      State = ledgeState,
      CanJump = true,
      Input = CreateInput(new PlayerInput { Jump =  true })
    });
  }

  static IEnumerable<TestCaseData> MovementTestCases() {
    Func<float, float, bool, bool, CharacterContext>
      context = (x, y, d, g) => new CharacterContext {
        State = new PlayerState { Direction = d },
        IsGrounded = g,
        Input = CreateInput(new PlayerInput { Movement = new Vector2(x, y) })
      };
    foreach (var dir in new[] {true, false}) {
      foreach (var src in new[] {"Idle", "Walk"}) {
        yield return TestCase(src, "Idle", context(0f, 0f, dir, true));
        yield return TestCase(src, "Walk", context(1f, 0f, dir, true));
      }
      foreach (var src in new[] {"Idle", "Walk", "CrouchStart", "Crouch", "CrouchEnd"}) {
        yield return TestCase(src, "Fall", context(0f, 0f, dir, false));
      }
      foreach (var src in new[] {"FallHelpless", "Fall", "EscapeAir"}) {
        yield return TestCase(src, "Land", context(0f, 0f, dir, true));
      }
      var direction = dir ? 1.0f : -1.0f;
      var directionState = new PlayerState { Direction = dir };
      yield return TestCase("Idle", "Dash", new CharacterContext {
        State = directionState,
        IsGrounded = true,
        Input = CreateInput(new PlayerInput { Smash = -Vector2.right })
      });
      yield return TestCase("Run", "RunTurn", new CharacterContext {
        State = directionState,
        IsGrounded = true,
        Input = CreateInput(new PlayerInput { Movement = Vector2.right * -direction })
      });
      yield return TestCase("Run", "Run", new CharacterContext {
        State = directionState,
        IsGrounded = true,
        Input = CreateInput(new PlayerInput { Movement = Vector2.right * direction })
      });
      foreach (var src in new [] {"Idle", "Walk", "Dash", "Run", "RunTurn", "RunBrake", "CrouchStart", "Crouch", "CrouchEnd", "Shield.Main"}) {
        yield return TestCase(src, "JumpStart", new CharacterContext {
          State = new PlayerState { Direction = dir },
          IsGrounded = true,
          CanJump = true,
          Input = CreateInput(new PlayerInput { Jump = true })
        });
      }
    }
  }

  static TestCaseData TestCase(string src, string dst, CharacterContext context) {
    return new TestCaseData(src, context).SetName($"{src} -> {dst}").Returns(dst);
  }

  static IEnumerable<TestCaseData> AttackTestCases() {
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
        yield return TestCase(src, "Neutral", context(0f, 0f, dir, true));
        yield return TestCase(src, "TiltUp", context(0f, magnitude, dir, true));
        yield return TestCase(src, "TiltDown", context(0f, -magnitude, dir, true));
        yield return TestCase(src, "TiltSide", context(magnitude, 0f, dir, true));
        yield return TestCase(src, "TiltSide", context(-magnitude, 0f, dir, true));
      }
    }
    foreach (var src in new[] {"Fall", "Jump", "JumpAerial"}) {
      foreach (var dir in new[] {true, false}) {
        yield return TestCase(src, "AerialNeutral", context(0f, 0f, dir, false));
        yield return TestCase(src, "AerialUp", context(0f, magnitude, dir, false));
        yield return TestCase(src, "AerialDown", context(0f, -magnitude, dir, false));
      }
      yield return TestCase(src, "AerialForward", context(magnitude, 0f, true, false));
      yield return TestCase(src, "AerialBackward", context(-magnitude, 0f, true, false));
      yield return TestCase(src, "AerialBackward", context(magnitude, 0f, false, false));
      yield return TestCase(src, "AerialForward", context(-magnitude, 0f, false, false));
    }
    // TODO(james7132): Dash Attacks
    // foreach (var src in new[] {"Dash", "Run", "RunTurn", "RunBrake"}) {
    //     foreach (var dir in new[] {true, false}) {
    //         yield return new object[] {src, "", context(magnitude, 0f, true, false)};
    //     }
    // }
  }

  static IEnumerable<TestCaseData> AutomaticTestCases() {
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
      yield return TestCase(kvp.Key, kvp.Value, new CharacterContext {
        State = new PlayerState { StateTick = 101 },
        StateLength = 100 * Time.fixedDeltaTime,
        CanJump = true,
        Input = CreateInput(new PlayerInput())
      });
    }
    yield return TestCase("TiltDown", "Crouch", new CharacterContext {
      State = new PlayerState { StateTick = 100 },
      StateLength = 100 * Time.fixedDeltaTime,
      Input = CreateInput(new PlayerInput { Movement = -Vector2.up })
    });
    yield return TestCase("Dash", "Run", new CharacterContext {
      State = new PlayerState { StateTick = 100 },
      StateLength = 100 * Time.fixedDeltaTime,
      Input = CreateInput(new PlayerInput { Movement = Vector2.right})
    });
  }

  string TestTransition(string src, CharacterContext context) {
    return _stateController.UpdateState(_stateMap[src], context).Name;
  }

  [Test, TestCaseSource("AutomaticTestCases")]
  public string automatic_exits(string src, CharacterContext context = null) {
    return TestTransition(src, context ?? new CharacterContext {
      State = new PlayerState {
        StateTick = 101,
        ShieldDamage = 100
      },
      StateLength = 100 * Time.fixedDeltaTime
    });
  }        

  [Test, TestCaseSource("AttackTestCases")]
  public string attack_transitions(string src, CharacterContext context) {
    return TestTransition(src, context);
  }

  [Test, TestCaseSource("MovementTestCases")]
  public string movement_transitions(string src,CharacterContext context) {
    return TestTransition(src, context);
  }

  [Test, TestCaseSource("LedgeTestCases")]
  public string ledge_transitions(string src, CharacterContext context) {
    return TestTransition(src, context);
  }

}