using UnityEditor;
using UnityEngine;

namespace TeamUtility.Editor.IO.InputManager {

    public static class MenuCommands {

        [MenuItem("Team Utility/Input Manager/Create Input Manager", false, 2)]
        private static void CreateInputManager() {
            var gameObject = new GameObject("Input Manager");
            gameObject.AddComponent<TeamUtility.IO.InputManager>();

            Selection.activeGameObject = gameObject;
        }

        [MenuItem("Team Utility/Input Manager/Documentation", false, 400)]
        public static void OpenDocumentationPage() {
            Application.OpenURL("https://github.com/daemon3000/InputManager/wiki");
        }

        [MenuItem("Team Utility/Input Manager/Report Bug", false, 401)]
        public static void OpenReportBugPage() {
            Application.OpenURL("https://github.com/daemon3000/InputManager/issues");
        }

        [MenuItem("Team Utility/Input Manager/Contact", false, 402)]
        public static void OpenContactDialog() {
            var message = "Email: geambasu.cristi@gmail.com";
            EditorUtility.DisplayDialog("Contact", message, "Close");
        }

        [MenuItem("Team Utility/Input Manager/Forum", false, 403)]
        public static void OpenForumPage() {
            Application.OpenURL("http://forum.unity3d.com/threads/223321-Free-Custom-Input-Manager");
        }

        [MenuItem("Team Utility/Input Manager/About", false, 404)]
        public static void OpenAboutDialog() {
            var message =
                "Input Manager, MIT licensed\nCopyright \u00A9 2015 Cristian Alexandru Geambasu\nhttps://github.com/daemon3000/InputManager";
            EditorUtility.DisplayDialog("About", message, "OK");
        }

    }

}