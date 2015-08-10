namespace UnityTest {

    public class UnitTestsRunnerSettings : ProjectSettingsBase {

        public bool autoSaveSceneBeforeRun;
        public bool horizontalSplit = true;
        public bool runOnRecompilation;
        public bool runTestOnANewScene;

        public void ToggleRunTestOnANewScene() {
            runTestOnANewScene = !runTestOnANewScene;
            Save();
        }

        public void ToggleAutoSaveSceneBeforeRun() {
            autoSaveSceneBeforeRun = !autoSaveSceneBeforeRun;
            Save();
        }

        public void ToggleHorizontalSplit() {
            horizontalSplit = !horizontalSplit;
            Save();
        }

    }

}