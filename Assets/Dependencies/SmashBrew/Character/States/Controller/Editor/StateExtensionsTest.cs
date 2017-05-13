using System;
using NUnit.Framework;

namespace HouraiTeahouse.SmashBrew.States {

    [Parallelizable]
    public class StateExtensionsTest {

        public class TestState : State<object> {
        }

        [Test]
        public void add_transition_predicate_throws_if_state_is_null() {
            Assert.Throws<ArgumentNullException>(() => new TestState().AddTransition<TestState, object>(null, ctx => true));
        }

        [Test]
        public void add_transition_predicate_throws_if_predicate_is_null() {
            Assert.Throws<ArgumentNullException>(() => new TestState().AddTransition(new TestState(), null));
        }

        [Test]
        public void add_transition_predicate_can_succeed() {
            var stateA = new TestState();
            var stateB = new TestState();
            stateA.AddTransition(stateB, ctx => ctx != null);
            Assert.AreEqual(stateB, stateA.EvaluateTransitions(new object()));
            Assert.AreEqual(null, stateA.EvaluateTransitions(null));
        }

        [Test]
        public void add_transition_multi_can_succeed() {
            var multistate = new[] {new TestState(), new TestState(), new TestState()};
            var target = new TestState();
            for (var i = 0; i < 10; i++) {
                multistate.AddTransitions<TestState, object>(ctx => target);
                foreach (TestState testState in multistate)
                    Assert.AreEqual(i + 1, testState.Transitions.Count);
            }
            foreach (TestState testState in multistate)
                Assert.AreEqual(target, testState.EvaluateTransitions(new object()));
        }

    }

}

