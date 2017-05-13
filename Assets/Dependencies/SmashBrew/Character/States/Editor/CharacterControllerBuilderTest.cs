using UnityEngine;
using NUnit.Framework;
using HouraiTeahouse.SmashBrew.States;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HouraiTeahouse.SmashBrew.Characters {

    [ParallelizableAttribute]
    public class CharacterControllerBuilderTest {

        StateController<CharacterState, CharacterStateContext> _stateController;
        Dictionary<string, CharacterState> _stateMap;

        [SetUp]
        public void Setup() {
            var instance = ScriptableObject.CreateInstance<CharacterControllerBuilder>();
            var builder = new StateControllerBuilder<CharacterState, CharacterStateContext>();
            _stateController = instance.BuildCharacterControllerImpl(builder);
            _stateMap = _stateController.States.ToDictionary(s => s.Name, s => s);
            UnityEngine.Object.DestroyImmediate(instance);
        }        

        static IEnumerable<object[]> AttackTestCases() {
            Func<float, float, bool, bool, CharacterStateContext>
                context = (x, y, d, g) => new CharacterStateContext {
                    IsGrounded = g,
                    Direction = d ? 1.0f : -1.0f,
                    Input = new InputContext {
                        Movement = new Vector2(x, y),
                        Attack = new ButtonContext {
                            LastFrame = false,
                            Current = true
                        },
                    }
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
                {"SmashUp.Attack", "Idle"},
                {"SmashSide.Attack", "Idle"},
                {"SmashDown.Attack", "Idle"},
                {"EscapeAir", "FallHelpless"},
                {"Escape", "Shield.Main"},
                {"EscapeForward", "Shield.Main"},
                {"EscapeBackward", "Shield.Main"},
                {"Dash", "Run"},
                {"RunTurn", "Run"},
                {"LedgeGrab", "LedgeIdle"},
                {"LedgeJump", "Jump"},
                {"JumpStart", "Jump"},
                {"JumpAerial", "Jump"},
                {"CrouchStart", "Crouch"},
            };
            return cases.Select(c => new object[] {c.Key, c.Value});
        }

        [Test, TestCaseSource("AutomaticTestCases")]
        public void automatic_exits(string src, string dst) {
            var _context = new CharacterStateContext {
                NormalizedAnimationTime = 1.0f,
                ShieldHP = 100
            };
           _stateController.SetState(_stateMap[src]);
            _stateController.UpdateState(_context);
            Assert.AreEqual(dst, _stateController.CurrentState.Name);
        }        

        [Test, TestCaseSource("AttackTestCases")]
        public void attack_transitions(string src, string dst, CharacterStateContext context) {
            _stateController.SetState(_stateMap[src]);
            _stateController.UpdateState(context);
            Assert.AreEqual(dst, _stateController.CurrentState.Name);
        }

    }
}

