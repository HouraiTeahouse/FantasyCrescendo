using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityTest.IntegrationTests;

namespace UnityTest {

    public static partial class Batch {

        private const string k_TestScenesParam = "-testscenes=";
        private const string k_TargetPlatformParam = "-targetPlatform=";
        private const string k_ResultFileDirParam = "-resultsFileDirectory=";

        public static void RunIntegrationTests() {
            BuildTarget? targetPlatform = GetTargetPlatform();
            List<string> sceneList = GetTestScenesList();
            if (sceneList.Count == 0)
                sceneList = FindTestScenesInProject();
            RunIntegrationTests(targetPlatform, sceneList);
        }

        public static void RunIntegrationTests(BuildTarget? targetPlatform) {
            List<string> sceneList = FindTestScenesInProject();
            RunIntegrationTests(targetPlatform, sceneList);
        }

        public static void RunIntegrationTests(BuildTarget? targetPlatform, List<string> sceneList) {
            if (targetPlatform.HasValue)
                BuildAndRun(targetPlatform.Value, sceneList);
            else
                RunInEditor(sceneList);
        }

        private static void BuildAndRun(BuildTarget target, List<string> sceneList) {
            string resultFilePath = GetParameterArgument(k_ResultFileDirParam);

            const int port = 0;
            List<string> ipList = TestRunnerConfigurator.GetAvailableNetworkIPs();

            var config = new PlatformRunnerConfiguration {
                buildTarget = target,
                scenes = sceneList.ToArray(),
                projectName = "IntegrationTests",
                resultsDir = resultFilePath,
                sendResultsOverNetwork = InternalEditorUtility.inBatchMode,
                ipList = ipList,
                port = port
            };

            if (Application.isWebPlayer) {
                config.sendResultsOverNetwork = false;
                Debug.Log(
                          "You can't use WebPlayer as active platform for running integration tests. Switching to Standalone");
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);
            }

            PlatformRunner.BuildAndRunInPlayer(config);
        }

        private static void RunInEditor(List<string> sceneList) {
            CheckActiveBuildTarget();

            NetworkResultsReceiver.StopReceiver();
            if (sceneList == null || sceneList.Count == 0) {
                Debug.Log("No scenes on the list");
                EditorApplication.Exit(returnCodeRunError);
                return;
            }
            EditorBuildSettings.scenes = sceneList.Select(s => new EditorBuildSettingsScene(s, true)).ToArray();
            EditorApplication.OpenScene(sceneList.First());
            GuiHelper.SetConsoleErrorPause(false);

            var config = new PlatformRunnerConfiguration {
                resultsDir = GetParameterArgument(k_ResultFileDirParam),
                ipList = TestRunnerConfigurator.GetAvailableNetworkIPs(),
                port = PlatformRunnerConfiguration.TryToGetFreePort(),
                runInEditor = true
            };

            var settings = new PlayerSettingConfigurator(true);
            settings.AddConfigurationFile(TestRunnerConfigurator.integrationTestsNetwork,
                                          string.Join("\n", config.GetConnectionIPs()));

            NetworkResultsReceiver.StartReceiver(config);

            EditorApplication.isPlaying = true;
        }

        private static void CheckActiveBuildTarget() {
            string[] notSupportedPlatforms = {"MetroPlayer", "WebPlayer", "WebPlayerStreamed"};
            if (notSupportedPlatforms.Contains(EditorUserBuildSettings.activeBuildTarget.ToString())) {
                Debug.Log("activeBuildTarget can not be  "
                          + EditorUserBuildSettings.activeBuildTarget +
                          " use buildTarget parameter to open Unity.");
            }
        }

        private static BuildTarget? GetTargetPlatform() {
            string platformString = null;
            BuildTarget buildTarget;
            foreach (string arg in Environment.GetCommandLineArgs()) {
                if (arg.ToLower().StartsWith(k_TargetPlatformParam.ToLower())) {
                    platformString = arg.Substring(k_ResultFilePathParam.Length);
                    break;
                }
            }
            try {
                if (platformString == null)
                    return null;
                buildTarget = (BuildTarget) Enum.Parse(typeof (BuildTarget), platformString);
            } catch {
                return null;
            }
            return buildTarget;
        }

        private static List<string> FindTestScenesInProject() {
            var integrationTestScenePattern = "*Test?.unity";
            return Directory.GetFiles("Assets", integrationTestScenePattern, SearchOption.AllDirectories).ToList();
        }

        private static List<string> GetTestScenesList() {
            List<string> sceneList = new List<string>();
            foreach (string arg in Environment.GetCommandLineArgs()) {
                if (arg.ToLower().StartsWith(k_TestScenesParam)) {
                    string[] scenesFromParam = arg.Substring(k_TestScenesParam.Length).Split(',');
                    foreach (string scene in scenesFromParam) {
                        string sceneName = scene;
                        if (!sceneName.EndsWith(".unity"))
                            sceneName += ".unity";
                        string[] foundScenes = Directory.GetFiles(Directory.GetCurrentDirectory(),
                                                                  sceneName,
                                                                  SearchOption.AllDirectories);
                        if (foundScenes.Length == 1)
                            sceneList.Add(foundScenes[0].Substring(Directory.GetCurrentDirectory().Length + 1));
                        else
                            Debug.Log(sceneName + " not found or multiple entries found");
                    }
                }
            }
            return sceneList.Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
        }

    }

}