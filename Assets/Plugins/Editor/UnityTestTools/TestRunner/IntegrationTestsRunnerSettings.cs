namespace UnityTest {

    public class IntegrationTestsRunnerSettings : ProjectSettingsBase {

        public bool blockUIWhenRunning = true;
        public bool pauseOnTestFailure;

        public void ToggleBlockUIWhenRunning() {
            blockUIWhenRunning = !blockUIWhenRunning;
            Save();
        }

        public void TogglePauseOnTestFailure() {
            pauseOnTestFailure = !pauseOnTestFailure;
            Save();
        }

    }

}