using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Hourai {

    public class PostBuildCopy : MonoBehaviour {

        private static string BuildDirectory {
            get { return Directory.GetCurrentDirectory() + "/Build"; }
        }

        /// <summary>
        /// Moves all extra files in the "Post Build" folder under the project's root directory into
        /// builds.
        /// </summary>
        /// <param name="target">the target platform Unity is building the game for.</param>
        /// <param name="path">the path at which the new build is being saved</param>
        [PostProcessBuild]
        static void AddPostBuildFiles(BuildTarget target, string path) {
            string copyPath= Directory.GetCurrentDirectory() + "/Post Build";
            path = Path.GetDirectoryName(path) + "/";
            
            switch (target) {
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
            Debug.Log(copyPath);

            // Copy all Post Build files to output directory

            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(copyPath,
                                                                "*",
                                                                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(copyPath, path));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(copyPath,
                                                          "*.*",
                                                          SearchOption.AllDirectories)) {
                if (!newPath.Contains(".meta"))
                    File.Copy(newPath, newPath.Replace(copyPath, path), true);
            }
        }

        /// <summary>
        /// Menu command to clear all PlayerPrefs
        /// </summary>
        [MenuItem("Hourai/Clear Player Prefs %#c")]
        static void ClearPlayerPrefs() {
            PlayerPrefs.DeleteAll();
            Debug.Log("HumanPlayer Prefs Cleared.");
        }

        /// <summary>
        /// Builds the current project for the Windows platform.
        /// Does both the 32 and 64 bit versions.
        /// </summary>
        [MenuItem("Hourai/Build Windows", false, 51)]
        static void BuildWindows() {
            string buildFolder = BuildDirectory + "/Windows/";
            Build(buildFolder, BuildTarget.StandaloneWindows);
            buildFolder = BuildDirectory + "/Windows_64/";
            Build(buildFolder, BuildTarget.StandaloneWindows64);
        }

        /// <summary>
        /// Builds the current project for the Mac platform.
        /// </summary>
        [MenuItem("Hourai/Build Mac", false, 51)]
        static void BuildMac() {
            string buildFolder = BuildDirectory + "/Mac/";
            Build(buildFolder, BuildTarget.StandaloneOSXUniversal);
        }

        /// <summary>
        /// Builds the current project for the Linux platform.
        /// </summary>
        [MenuItem("Hourai/Build Linux", false, 51)]
        static void BuildLinux() {
            string buildFolder = BuildDirectory + "/Linux/";
            Build(buildFolder, BuildTarget.StandaloneLinuxUniversal);
        }

        /// <summary>
        /// Buidls the current project for Windows, Mac, and Linux.
        /// </summary>
        [MenuItem("Hourai/Build All", false, 101)]
        static void BuildAll() {
            BuildWindows();
            BuildMac();
            BuildLinux();
        }

        /// <summary>
        /// Builds the current project for a specific target platform.
        /// </summary>
        /// <param name="path">the target path for where to store the new build</param>
        /// <param name="target">the target platform to build the project for</param>
        public static void Build(string path, BuildTarget target) {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // Delete everything in build folder
            var directory = new DirectoryInfo(path);
            foreach (FileInfo file in directory.GetFiles())
                file.Delete();
            foreach (DirectoryInfo subdirectory in directory.GetDirectories())
                subdirectory.Delete(true);

            string executablePath = path + "fantasyHourai";
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
    }

}
