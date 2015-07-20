using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

namespace Genso.API.Editor {

    public class GensoEditor
    {
        private static string root = Regex.Replace(Application.dataPath, "Assets", "");

        [MenuItem("Assets/Create/Game Config")]
        public static void CreateConfig()
        {
            CreateAsset<Config>();
        }

        [MenuItem("Assets/Create/Character Data")]
        public static void CreateCharacterData() {
            CreateAsset<CharacterData>();
        }

        [MenuItem("Genso/Clear Player Prefs %#c")]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Player Prefs Cleared.");
        }

        [MenuItem("Genso/Build Windows", false, 51)]
        public static void BuildWindows()
        {
            string buildFolder = root + "/Build/Windows/";
            Build(buildFolder, BuildTarget.StandaloneWindows);
            buildFolder = root + "/Build/Windows_64/";
            Build(buildFolder, BuildTarget.StandaloneWindows64);
        }

        [MenuItem("Genso/Build Mac", false, 51)]
        public static void BuildMac()
        {
            string buildFolder = root + "/Build/Mac/";
            Build(buildFolder, BuildTarget.StandaloneOSXUniversal);
        }

        [MenuItem("Genso/Build Linux", false, 51)]
        public static void BuildLinux() {
            string buildFolder = root + "/Build/Linux/";
            Build(buildFolder, BuildTarget.StandaloneLinuxUniversal);
        }

        [MenuItem("Genso/Build All", false, 101)]
        public static void BuildAll() {
            BuildWindows();
            BuildMac();
            BuildLinux();
        }

        public static void Build(string path, BuildTarget target) {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            // Delete everything in build folder
            DirectoryInfo directory = new DirectoryInfo(path);
            foreach(FileInfo file in directory.GetFiles())
                file.Delete();
            foreach(DirectoryInfo subdirectory in directory.GetDirectories())
                subdirectory.Delete(true);

            path += "genso";
            switch (target) {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    path += ".exe";
                    break;
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
                    path += ".x86";
                    break;
            }

            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            string[] scenePaths = new string[scenes.Length];
            for (var i = 0; i < scenes.Length; i++)
                scenePaths[i] = scenes[i].path;

            BuildPipeline.BuildPlayer(scenePaths, path, target, BuildOptions.None);
        }

        public static void CopyAll(string startPath, string endPath) {
            
        }

        /// <summary>
        /// Create new asset from <see cref="ScriptableObject"/> type with unique name at
        /// selected folder in project window. Asset creation can be cancelled by pressing
        /// escape key when asset is initially being named.
        /// </summary>
        /// <typeparam name="T">Type of scriptable object.</typeparam>
        public static void CreateAsset<T>() where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
            ProjectWindowUtil.CreateAsset(asset, "New " + typeof(T).Name + ".asset");
        }
    }


}