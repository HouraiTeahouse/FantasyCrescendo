using UnityEngine;

namespace UnityTest {

    public static class Assertions {

        public static void CheckAssertions() {
            AssertionComponent[] assertions =
                Object.FindObjectsOfType(typeof (AssertionComponent)) as AssertionComponent[];
            CheckAssertions(assertions);
        }

        public static void CheckAssertions(AssertionComponent assertion) {
            CheckAssertions(new[] {assertion});
        }

        public static void CheckAssertions(GameObject gameObject) {
            CheckAssertions(gameObject.GetComponents<AssertionComponent>());
        }

        public static void CheckAssertions(AssertionComponent[] assertions) {
            if (!Debug.isDebugBuild)
                return;
            foreach (AssertionComponent assertion in assertions) {
                assertion.checksPerformed++;
                bool result = assertion.Action.Compare();
                if (!result) {
                    assertion.hasFailed = true;
                    assertion.Action.Fail(assertion);
                }
            }
        }

    }

}