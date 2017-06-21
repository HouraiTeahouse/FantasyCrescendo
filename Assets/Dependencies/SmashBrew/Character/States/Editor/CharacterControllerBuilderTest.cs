using UnityEngine;
using NUnit.Framework;
using HouraiTeahouse.SmashBrew.States;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HouraiTeahouse.SmashBrew.Characters {

    [Parallelizable]
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

        static IEnumerable<object[]> LedgeTestCases() {
            Func<float, float, bool, bool, CharacterStateContext>
                context = (x, y, l, a) => {
                    var ctx = new CharacterStateContext {
                        NormalizedAnimationTime = 1.0f,
                        IsGrabbingLedge = l,
                    };
                    var input = new InputContext { Movement = new Vector2(x, y) };
                    if (a)
                        input.Attack = new ButtonContext { LastFrame = false, Current = true };
                    ctx.Input = input;
                    return ctx;
                };
            yield return new object[] { "Idle", "LedgeGrab", new CharacterStateContext { 
                IsGrabbingLedge = true,
                IsGrounded = true
            }};
            foreach (var src in new[] {"Fall", "FallHelpless"}) {
                yield return new object[] { src, "LedgeGrab", new CharacterStateContext { 
                    IsGrabbingLedge = true,
                    IsGrounded = false
                }};
            }
            yield return new object[] {"LedgeGrab", "LedgeIdle", new CharacterStateContext { NormalizedAnimationTime = 1.0f }};
            yield return new object[] {"LedgeIdle", "LedgeRelease", new CharacterStateContext {
                Input = new InputContext { Movement = new Vector2(0, -1) }
            }};
            yield return new object[] {"LedgeIdle", "LedgeAttack", new CharacterStateContext {
                Input = new InputContext { Attack = new ButtonContext { LastFrame = false, Current = true }}
            }};
            yield return new object[] {"LedgeIdle", "LedgeJump", new CharacterStateContext {
                Input = new InputContext { Jump = new ButtonContext { LastFrame = false, Current = true }}
            }};
        }

        static IEnumerable<object[]> MovementTestCases() {
            Func<float, float, bool, bool, CharacterStateContext>
                context = (x, y, d, g) => new CharacterStateContext {
                    IsGrounded = g,
                    Direction = d ? 1.0f : -1.0f,
                    Input = new InputContext {
                        Movement = new Vector2(x, y),
                    }
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
                yield return new object[] {"Idle", "Dash", new CharacterStateContext {
                    Direction = direction,
                    IsGrounded = true,
                    Input = new InputContext {
                        Smash = new Vector2(-1, 0)
                    }
                }};
                yield return new object[] {"Run", "RunTurn", new CharacterStateContext {
                    Direction = direction,
                    IsGrounded = true,
                    Input = new InputContext {
                        Movement = new Vector2(-direction, 0f)
                    }
                }};
                yield return new object[] {"Run", "Run", new CharacterStateContext {
                    Direction = direction,
                    IsGrounded = true,
                    Input = new InputContext {
                        Movement = new Vector2(direction, 0)
                    }
                }};
                foreach (var src in new [] {"Idle", "Walk", "Dash", "Run", "RunTurn", "RunBrake", "CrouchStart", "Crouch", "CrouchEnd", "Shield.Main"}) {
                    yield return new object[] {src, "JumpStart", new CharacterStateContext {
                        Direction = dir ? 1.0f : -1.0f,
                        IsGrounded = true,
                        Input = new InputContext {
                            Jump = new ButtonContext {
                                LastFrame = false,
                                Current = true
                            }
                        }
                    }};
                }
            }
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
                yield return new[] {kvp.Key, kvp.Value, null};
            }
            yield return new object[] {"Dash", "Run", new CharacterStateContext {
                NormalizedAnimationTime = 1.0f,
                Input = new InputContext {
                    Movement = new Vector2(1.0f, 0.0f)
                }
            }};
        }

        void TestTransition(string src, string dst, CharacterStateContext context) {
            _stateController.SetState(_stateMap[src]);
            _stateController.UpdateState(context);
            Assert.AreEqual(dst, _stateController.CurrentState.Name);
        }

        [Test, TestCaseSource("AutomaticTestCases")]
        public void automatic_exits(string src, string dst, CharacterStateContext context = null) {
            TestTransition(src, dst, context ?? new CharacterStateContext {
                NormalizedAnimationTime = 1.0f,
                ShieldHP = 100
            });
        }        

        [Test, TestCaseSource("AttackTestCases")]
        public void attack_transitions(string src, string dst, CharacterStateContext context) {
            TestTransition(src, dst, context);
        }

        [Test, TestCaseSource("MovementTestCases")]
        public void movement_transitions(string src, string dst, CharacterStateContext context) {
            TestTransition(src, dst, context);
        }

        [Test, TestCaseSource("LedgeTestCases")]
        public void ledge_transitions(string src, string dst, CharacterStateContext context) {
            TestTransition(src, dst, context);
        }
    }
}

