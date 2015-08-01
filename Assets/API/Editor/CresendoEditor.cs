using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Crescendo.API.Editor {

    public static class CrescendoEditor
    {
        private static string buildRoot = Regex.Replace(Application.dataPath, "Assets", "") + "/Build/";
        private static string copyRoot = Application.dataPath + "/Post Build/";

        [MenuItem("Assets/Create/Game Config")]
        public static void CreateConfig()
        {
            CreateAsset<Config>();
        }

        [MenuItem("Assets/Create/Character Data")]
        public static void CreateCharacterData() {
            CreateAsset<CharacterData>();
        }

        [MenuItem("Assets/Create/BGM Group")]
        public static void CreateStageData() {
            CreateAsset<BGMGroup>();
        }


        [MenuItem("Crescendo/Clear Player Prefs %#c")]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Player Prefs Cleared.");
        }

        [MenuItem("Crescendo/Build Windows", false, 51)]
        public static void BuildWindows()
        {
            string buildFolder = buildRoot + "Windows/";
            Build(buildFolder, BuildTarget.StandaloneWindows);
            buildFolder = buildRoot + "Windows_64/";
            Build(buildFolder, BuildTarget.StandaloneWindows64);
        }

        [MenuItem("Crescendo/Build Mac", false, 51)]
        public static void BuildMac()
        {
            string buildFolder = buildRoot + "Mac/";
            Build(buildFolder, BuildTarget.StandaloneOSXUniversal);
        }

        [MenuItem("Crescendo/Build Linux", false, 51)]
        public static void BuildLinux() {
            string buildFolder = buildRoot + "Linux/";
            Build(buildFolder, BuildTarget.StandaloneLinuxUniversal);
        }

        [MenuItem("Crescendo/Build All", false, 101)]
        public static void BuildAll() {
            BuildWindows();
            BuildMac();
            BuildLinux();
        }

		[PostProcessBuild]
		private static void AddPostBuildFiles(BuildTarget target, string path) {

			path = Path.GetDirectoryName(path) + "/";
			string copyPath = copyRoot;
			
			switch(target) {
				case BuildTarget.StandaloneWindows:
				case BuildTarget.StandaloneWindows64:
					copyPath += "Windows/";
					break;
				case BuildTarget.StandaloneOSXIntel:
				case BuildTarget.StandaloneOSXIntel64:
				case BuildTarget.StandaloneOSXUniversal:
					copyPath += "Mac/";
					break;
				case BuildTarget.StandaloneLinux:
				case BuildTarget.StandaloneLinux64:
				case BuildTarget.StandaloneLinuxUniversal:
					copyPath += "Linux/";
					break;
				default:
					return;
			}
			Debug.Log (copyPath);

			// Copy all Post Build files to output directory
			
			//Now Create all of the directories
			foreach (string dirPath in Directory.GetDirectories(copyPath, "*", 
			                                                    SearchOption.AllDirectories))
				Directory.CreateDirectory(dirPath.Replace(copyPath, path));
			
			//Copy all the files & Replaces any files with the same name
			foreach (string newPath in Directory.GetFiles(copyPath,
			                                              "*.*",
			                                              SearchOption.AllDirectories)) {
				if(!newPath.Contains(".meta"))
					File.Copy(newPath, newPath.Replace(copyPath, path), true);   
			}
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

            string executablePath = path + "fantasycrescendo";
            switch (target) {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    executablePath += ".exe";
                    break;
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
                    executablePath += ".x86";
                    break;
            }

            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            string[] scenePaths = new string[scenes.Length];
            for (var i = 0; i < scenes.Length; i++)
                scenePaths[i] = scenes[i].path;

            BuildPipeline.BuildPlayer(scenePaths, executablePath, target, BuildOptions.None);
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
