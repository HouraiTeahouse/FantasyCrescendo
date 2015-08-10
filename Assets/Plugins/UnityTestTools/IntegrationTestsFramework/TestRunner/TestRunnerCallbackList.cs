using System.Collections.Generic;

namespace UnityTest.IntegrationTestRunner {

    public class TestRunnerCallbackList : ITestRunnerCallback {

        private readonly List<ITestRunnerCallback> m_CallbackList = new List<ITestRunnerCallback>();

        public void RunStarted(string platform, List<TestComponent> testsToRun) {
            foreach (ITestRunnerCallback unitTestRunnerCallback in m_CallbackList)
                unitTestRunnerCallback.RunStarted(platform, testsToRun);
        }

        public void RunFinished(List<TestResult> testResults) {
            foreach (ITestRunnerCallback unitTestRunnerCallback in m_CallbackList)
                unitTestRunnerCallback.RunFinished(testResults);
        }

        public void TestStarted(TestResult test) {
            foreach (ITestRunnerCallback unitTestRunnerCallback in m_CallbackList)
                unitTestRunnerCallback.TestStarted(test);
        }

        public void TestFinished(TestResult test) {
            foreach (ITestRunnerCallback unitTestRunnerCallback in m_CallbackList)
                unitTestRunnerCallback.TestFinished(test);
        }

        public void TestRunInterrupted(List<ITestComponent> testsNotRun) {
            foreach (ITestRunnerCallback unitTestRunnerCallback in m_CallbackList)
                unitTestRunnerCallback.TestRunInterrupted(testsNotRun);
        }

        public void Add(ITestRunnerCallback callback) {
            m_CallbackList.Add(callback);
        }

        public void Remove(ITestRunnerCallback callback) {
            m_CallbackList.Remove(callback);
        }

    }

}