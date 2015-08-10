using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace UnityTest {

    internal class PlayerSettingConfigurator {

        private const string k_TempPath = "Temp";
        private readonly string m_ProjectResourcesPath = Path.Combine("Assets", "Resources");
        private readonly bool m_Temp;
        private readonly List<string> m_TempFileList = new List<string>();
        private ResolutionDialogSetting m_DisplayResolutionDialog;
        private bool m_FullScreen;
        private bool m_ResizableWindow;
        private bool m_RunInBackground;

        public PlayerSettingConfigurator(bool saveInTempFolder) {
            m_Temp = saveInTempFolder;
        }

        private string resourcesPath {
            get { return m_Temp ? k_TempPath : m_ProjectResourcesPath; }
        }

        public void ChangeSettingsForIntegrationTests() {
            m_DisplayResolutionDialog = PlayerSettings.displayResolutionDialog;
            PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;

            m_RunInBackground = PlayerSettings.runInBackground;
            PlayerSettings.runInBackground = true;

            m_FullScreen = PlayerSettings.defaultIsFullScreen;
            PlayerSettings.defaultIsFullScreen = false;

            m_ResizableWindow = PlayerSettings.resizableWindow;
            PlayerSettings.resizableWindow = true;
        }

        public void RevertSettingsChanges() {
            PlayerSettings.defaultIsFullScreen = m_FullScreen;
            PlayerSettings.runInBackground = m_RunInBackground;
            PlayerSettings.displayResolutionDialog = m_DisplayResolutionDialog;
            PlayerSettings.resizableWindow = m_ResizableWindow;
        }

        public void AddConfigurationFile(string fileName, string content) {
            bool resourcesPathExists = Directory.Exists(resourcesPath);
            if (!resourcesPathExists)
                AssetDatabase.CreateFolder("Assets", "Resources");

            string filePath = Path.Combine(resourcesPath, fileName);
            File.WriteAllText(filePath, content);

            m_TempFileList.Add(filePath);
        }

        public void RemoveAllConfigurationFiles() {
            foreach (string filePath in m_TempFileList)
                AssetDatabase.DeleteAsset(filePath);
            if (Directory.Exists(resourcesPath)
                && Directory.GetFiles(resourcesPath).Length == 0)
                AssetDatabase.DeleteAsset(resourcesPath);
        }

    }

}