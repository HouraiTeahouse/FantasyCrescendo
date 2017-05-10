using System;
using NUnit.Framework;

namespace HouraiTeahouse.SmashBrew.States {

    public class StateControllerTest {

        public class TestState : State<TestContext> {
        }

        public class TestContext {
            public bool A;
            public int B;
            public float C;
        }

        readonly TestState A = new TestState();
        readonly TestState B = new TestState();
        readonly TestState C = new TestState();
        readonly TestState D = new TestState();
        readonly TestState E = new TestState();
        readonly TestState F = new TestState();
        StateController<TestState, TestContext> _controller;

        [SetUp]
        public void Setup() {
            var builder =
                new StateControllerBuilder<TestState, TestContext>()
                    .WithDefaultState(A)
                    .AddState(B)
                    .AddState(C)
                    .AddState(D)
                    .AddState(E)
                    .AddState(F);
            A.AddTransition(B, ctx => ctx.B > 10);
            A.AddTransition(C, ctx => ctx.C > 10);
            B.AddTransition(C, ctx => ctx.B > 20);
            B.AddTransition(D, ctx => ctx.C > 20);
            C.AddTransition(D, ctx => ctx.B > 30);
            C.AddTransition(E, ctx => ctx.C > 30);
            D.AddTransition(E, ctx => ctx.B > 40);
            D.AddTransition(F, ctx => ctx.C > 40);
            E.AddTransition(F, ctx => ctx.B > 50);
            E.AddTransition(A, ctx => ctx.C > 50);
            F.AddTransition(A, ctx => ctx.B > 50);
            F.AddTransition(B, ctx => ctx.C > 50);
            new[] {B, C, D, E, F}.AddTransition(A, ctx => ctx.A);
            _controller = builder.Build();
        }

        [Test]
        public void reset_restores_default_state() {
            _controller.ResetState();
            Assert.AreEqual(_controller.DefaultState, _controller.CurrentState);
        }

        [Test]
        public void set_state_throws_on_null_state() {
            Assert.Throws<ArgumentNullException>(() => _controller.SetState(null));
        }

        [Test]
        public void set_state_throws_on_invalid_state() {
            var nonState = new TestState();
            Assert.Throws<ArgumentException>(() => _controller.SetState(nonState));
        }

        [Test]
        public void set_state_forces_state_change() {
            var states = new[] {B, C, D, E, F};
            var called = false;
            _controller.OnStateChange += (b, a) => called = true;
            foreach (TestState testState in states) {
                _controller.SetState(testState);
                Assert.AreEqual(testState, _controller.CurrentState);
                Assert.IsTrue(called);
                called = false;
            }
        }

        [Test]
        public void update_follows_state_graph() {
            Assert.AreEqual(A, _controller.UpdateState(new TestContext()));
            Assert.AreEqual(A, _controller.UpdateState(new TestContext()));
            Assert.AreEqual(B, _controller.UpdateState(new TestContext { B = 100 }));
            Assert.AreEqual(C, _controller.UpdateState(new TestContext { B = 100 }));
            Assert.AreEqual(D, _controller.UpdateState(new TestContext { B = 100 }));
            Assert.AreEqual(E, _controller.UpdateState(new TestContext { B = 100 }));
            Assert.AreEqual(F, _controller.UpdateState(new TestContext { B = 100 }));
            Assert.AreEqual(A, _controller.UpdateState(new TestContext { B = 100 }));
            var states = new[] {B, C, D, E, F};
            foreach (TestState testState in states) {
                _controller.SetState(testState);
                Assert.AreEqual(A, _controller.UpdateState(new TestContext { A = true }));
            }
            _controller.SetState(A);
            Assert.AreEqual(C, _controller.UpdateState(new TestContext { C = 100f }));
            Assert.AreEqual(E, _controller.UpdateState(new TestContext { C = 100f }));
            Assert.AreEqual(A, _controller.UpdateState(new TestContext { C = 100f }));
            _controller.SetState(B);
            Assert.AreEqual(D, _controller.UpdateState(new TestContext { C = 100f }));
            Assert.AreEqual(F, _controller.UpdateState(new TestContext { C = 100f }));
            Assert.AreEqual(B, _controller.UpdateState(new TestContext { C = 100f }));
        }

    }

}
