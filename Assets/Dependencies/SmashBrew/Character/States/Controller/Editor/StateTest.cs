using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

namespace HouraiTeahouse.SmashBrew.States {

    public class StateTest {

        public class TestContext {
            public bool A { get; set; }
            public bool B { get; set; }
        }

        public class TestState : State<TestContext> {
        }

        [Test]
        public void state_add_transition_throws_if_null() {
            Assert.Throws<ArgumentNullException>(() => new TestState().AddTransition(null));
        }

        [Test]
        public void state_add_transition_throws_if_null_predicate() {
            Assert.Throws<ArgumentNullException>(() => new TestState().AddTransition(null, ctx => false));
        }

        [Test]
        public void state_null_transition_defers_to_later_transitions() {
            var stateA = new TestState();
            var stateB = new TestState();
            stateA.AddTransition(ctx => null);
            stateA.AddTransition(ctx => stateB);
            Assert.AreEqual(stateB, stateA.EvaluateTransitions(new TestContext()));
        }

        [Test]
        public void state_evaluate_transitions_prefers_earlier_transitions() {
            var stateA = new TestState();
            var stateB = new TestState();
            var stateC = new TestState();
            stateA.AddTransition(ctx => stateB);
            stateA.AddTransition(ctx => stateC);
            Assert.AreEqual(stateB, stateA.EvaluateTransitions(new TestContext()));
        }

        [Test]
        public void state_evaluate_transitions_depends_on_value() {
            var stateA = new TestState();
            var stateB = new TestState();
            var stateC = new TestState();
            stateA.AddTransition(ctx => {
                if (ctx.A)
                    return stateA;
                return ctx.B ? stateB : null;
            });
            stateA.AddTransition(ctx => stateC);
            Assert.AreEqual(stateC, stateA.EvaluateTransitions(new TestContext()));
            Assert.AreEqual(stateA, stateA.EvaluateTransitions(new TestContext { A = true }));
            Assert.AreEqual(stateB, stateA.EvaluateTransitions(new TestContext { B = true }));
        }

    }

}

