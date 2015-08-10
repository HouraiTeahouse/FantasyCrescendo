using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityTest.UnitTestRunner;

namespace UnityTest {

    public static partial class Batch {

        private const string k_ResultFilePathParam = "-resultFilePath=";
        private const string k_TestFilterParam = "-filter=";
        private const string k_CategoryParam = "-categories=";
        private const string k_DefaultResultFileName = "UnitTestResults.xml";
        public static int returnCodeTestsOk = 0;
        public static int returnCodeTestsFailed = 2;
        public static int returnCodeRunError = 3;

        public static void RunUnitTests() {
            PlayerSettings.useMacAppStoreValidation = false;
            TestFilter filter = GetTestFilter();
            string resultFilePath = GetParameterArgument(k_ResultFilePathParam) ?? Directory.GetCurrentDirectory();
            if (Directory.Exists(resultFilePath))
                resultFilePath = Path.Combine(resultFilePath, k_DefaultResultFileName);
            EditorApplication.NewScene();
            var engine = new NUnitTestEngine();
            UnitTestResult[] results;
            string[] categories;
            engine.GetTests(out results, out categories);
            engine.RunTests(filter, new TestRunnerEventListener(resultFilePath, results.ToList()));
        }

        private static TestFilter GetTestFilter() {
            string[] testFilterArg = GetParameterArgumentArray(k_TestFilterParam);
            string[] testCategoryArg = GetParameterArgumentArray(k_CategoryParam);
            var filter = new TestFilter {
                names = testFilterArg,
                categories = testCategoryArg
            };
            return filter;
        }

        private static string[] GetParameterArgumentArray(string parameterName) {
            string arg = GetParameterArgument(parameterName);
            if (string.IsNullOrEmpty(arg))
                return null;
            return arg.Split(',').Select(s => s.Trim()).ToArray();
        }

        private static string GetParameterArgument(string parameterName) {
            foreach (string arg in Environment.GetCommandLineArgs()) {
                if (arg.ToLower().StartsWith(parameterName.ToLower()))
                    return arg.Substring(parameterName.Length);
            }
            return null;
        }

        private class TestRunnerEventListener : ITestRunnerCallback {

            private readonly string m_ResultFilePath;
            private readonly List<UnitTestResult> m_Results;

            public TestRunnerEventListener(string resultFilePath, List<UnitTestResult> resultList) {
                m_ResultFilePath = resultFilePath;
                m_Results = resultList;
            }

            public void TestFinished(ITestResult test) {
                m_Results.Single(r => r.Id == test.Id).Update(test, false);
            }

            public void RunFinished() {
                string resultDestiantion = Application.dataPath;
                if (!string.IsNullOrEmpty(m_ResultFilePath))
                    resultDestiantion = m_ResultFilePath;
                string fileName = Path.GetFileName(resultDestiantion);
                if (!string.IsNullOrEmpty(fileName))
                    resultDestiantion = resultDestiantion.Substring(0, resultDestiantion.Length - fileName.Length);
                else
                    fileName = "UnitTestResults.xml";
#if !UNITY_METRO
                var resultWriter = new XmlResultWriter("Unit Tests", "Editor", m_Results.ToArray());
                resultWriter.WriteToFile(resultDestiantion, fileName);
#endif
                IEnumerable<UnitTestResult> executed = m_Results.Where(result => result.Executed);
                if (!executed.Any()) {
                    EditorApplication.Exit(returnCodeRunError);
                    return;
                }
                IEnumerable<UnitTestResult> failed = executed.Where(result => !result.IsSuccess);
                EditorApplication.Exit(failed.Any() ? returnCodeTestsFailed : returnCodeTestsOk);
            }

            public void TestStarted(string fullName) {}
            public void RunStarted(string suiteName, int testCount) {}

            public void RunFinishedException(Exception exception) {
                EditorApplication.Exit(returnCodeRunError);
                throw exception;
            }

        }

    }

}