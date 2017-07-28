using System;
using NUnit.Framework;
using System.Linq;

namespace HouraiTeahouse.SmashBrew.States {

    [Parallelizable]
    public class StateControllerBiulderTest {

        public class TestContext {
        }

        public class TestState : State<TestContext> {
        }

        TestState _state;
        StateControllerBuilder<TestState, TestContext> builder;
        StateControllerBuilder<TestState, TestContext> nonEmptyBuilder;

        [SetUp]
        public void Setup() {
            _state = new TestState();
            builder = new StateControllerBuilder<TestState, TestContext>();
            nonEmptyBuilder = new StateControllerBuilder<TestState, TestContext>();
            nonEmptyBuilder.AddState(_state);
        }

        [Test]
        public void state_controller_adds_state_successfully() {
            var count = builder.States.Count();
            var nonEmptyCount = nonEmptyBuilder.States.Count();
            Assert.AreEqual(0, count);
            Assert.AreEqual(1, nonEmptyCount);
            builder.AddState(new TestState());
            nonEmptyBuilder.AddState(new TestState());
            count = builder.States.Count();
            nonEmptyCount = nonEmptyBuilder.States.Count();
            Assert.AreEqual(1, count);
            Assert.AreEqual(2, nonEmptyCount);
        }

        [Test]
        public void state_controller_add_state_throws_if_non_unique() {
            Assert.DoesNotThrow(() => builder.AddState(_state));
            Assert.Throws<ArgumentException>(() => builder.AddState(_state));
            Assert.Throws<ArgumentException>(() => nonEmptyBuilder.AddState(_state));
        }

        [Test]
        public void state_controller_add_state_throws_if_null() {
            Assert.Throws<ArgumentNullException>(() => builder.AddState(null));
        }

        [Test]
        public void state_controller_build_fails_without_default_state() {
            Assert.Throws<InvalidOperationException>(() => builder.Build());
            Assert.Throws<InvalidOperationException>(() => builder.Build());
            builder.WithDefaultState(new TestState());
            nonEmptyBuilder.WithDefaultState(new TestState());
            Assert.DoesNotThrow(() => builder.Build());
            Assert.DoesNotThrow(() => builder.Build());
        }

    }

}

