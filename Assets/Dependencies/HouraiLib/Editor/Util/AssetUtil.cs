using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.Editor {

    /// <summary>
    /// Utility class for handling Assets in the Unity Editor.
    /// </summary>
    [InitializeOnLoad]
    public static class AssetUtil {

        static readonly Dictionary<string, string> DelayedMoves;
        const string ResourcePath = "Resources/";
        const string ResourceRegex = ".*/Resources/(.*?)\\..*";

        static AssetUtil() {
            DelayedMoves = new Dictionary<string, string>();
            EditorApplication.update += Update;
        }
        
        static void Update() {
            if (DelayedMoves.Count <= 0)
                return;

            var toRemove = new List<string>();

            foreach (KeyValuePair<string, string> pair in DelayedMoves) {
                string result = AssetDatabase.ValidateMoveAsset(pair.Key, pair.Value);
                if (!string.IsNullOrEmpty(result))
                    continue;
                AssetDatabase.MoveAsset(pair.Key, pair.Value);
                toRemove.Add(pair.Key);
            }

            foreach (string key in toRemove)
                DelayedMoves.Remove(key);
        }

        /// <summary>
        /// Create new asset from <see cref="ScriptableObject"/> type with unique name at
        /// selected folder in project window. Asset creation can be cancelled by pressing
        /// escape key when asset is initially being named.
        /// </summary>
        /// <typeparam name="T">Type of scriptable object.</typeparam>
        public static T CreateAssetInProjectWindow<T>(T asset = null) where T : ScriptableObject {
            if(asset == null)
                asset = ScriptableObject.CreateInstance<T>();
            ProjectWindowUtil.CreateAsset(asset, string.Format("New {0}.asset", typeof(T).Name));
            return asset;
        }

        public static string GetAssetFolderPath(Object asset) {
            return Regex.Replace(AssetDatabase.GetAssetPath(Check.NotNull(asset)), "(Assets)+/(.*)/.*?\\..*", "$2");
        }
        
        public static bool IsAsset(this Object obj) {
            return obj != null && AssetDatabase.Contains(obj);
        }

        public static string CreateAssetPath(params string[] folderNames) {
            Check.NotNull(folderNames);
            if (folderNames.Length <= 0)
                return string.Empty;

            var builder = new StringBuilder();
            builder.Append(folderNames[0]);
            for (var i = 1; i < folderNames.Length; i++) {
                builder.Append('/');
                builder.Append(folderNames[i]);
            }
            return builder.ToString();
        }

        public static bool IsValidFolder(string path) {
            return AssetDatabase.IsValidFolder("Assets/" + path);
        }

        public static void CreateAsset(string folder, Object obj, string suffix = null) {
            Check.NotNull(folder);
            Check.NotNull(obj);
            if (string.IsNullOrEmpty(suffix))
                suffix = "asset";
            if (obj.IsAsset())
                return;
            // Create folder if it doesn't already exist
            CreateFolder(folder);
            AssetDatabase.CreateAsset(obj, string.Format("Assets/{0}/{1}.{2}", folder, obj.name, suffix));
        }

        public static void MoveAsset(string targetFolder, Object asset) {
            Check.NotNull(targetFolder);
            Check.NotNull(asset);
            if (!asset.IsAsset()) {
                var gameObject = asset as GameObject;
                var component = asset as Component;
                if (component != null)
                    gameObject = component.gameObject;
                if (gameObject != null) {
                    // Assign asset to the object's prefab
                    Debug.Log("Is a GameObject, extracting Prefab...");
                    asset = asset.GetPrefab();
                }
            }

            if (!asset.IsAsset()) return;
            string assetPath = AssetDatabase.GetAssetPath(asset);

            // Create the folder if it doesn't already exist
            CreateFolder(targetFolder);

            string destination = "Assets/" + targetFolder + "/" + Path.GetFileName(assetPath);
            string result = AssetDatabase.ValidateMoveAsset(assetPath, destination);

            if (string.IsNullOrEmpty(result))
                AssetDatabase.MoveAsset(assetPath, destination);
            else
                DelayedMoves.Add(assetPath, destination);
        }

        public static IEnumerable<string> FindAssetGUIDs<T>(string nameFilter = null) where T : Object {
            return
                from guid in
                    AssetDatabase.FindAssets(string.Format(string.IsNullOrEmpty(nameFilter) ? "t: {0}" : "t:{0} {1}",
                        typeof (T).Name, nameFilter))
                select guid;
        } 

        public static IEnumerable<string> FindAssetPaths<T>(string nameFilter = null) where T : Object {
            return from guid in FindAssetGUIDs<T>()
                select AssetDatabase.GUIDToAssetPath(guid);
        } 

        public static T LoadFirstOrDefault<T>(string nameFilter = null) where T : Object {
            return AssetDatabase.LoadAssetAtPath<T>(FindAssetPaths<T>(nameFilter).FirstOrDefault());
        }

        public static T LoadFirstOrCreate<T>(string nameFilter = null) where T : ScriptableObject {
            var loaded = LoadFirstOrDefault<T>();
            return loaded ?? CreateAssetInProjectWindow<T>();
        }

        public static bool IsResourcePath(string path) {
            return !string.IsNullOrEmpty(path) && path.Contains(ResourcePath);
        }

        public static bool IsResource(Object asset) {
            return IsResourcePath(AssetDatabase.GetAssetPath(asset));
        }

        public static string GetResourcePath(Object asset) {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            if (!IsResourcePath(assetPath))
                return string.Empty;
            return Regex.Replace(assetPath, ResourceRegex, "$1");
        }

        public static void CreateFolder(string path) {
            Check.NotNull(path);
            if (IsValidFolder(path))
                return;

            string[] folders = path.Split('/');
            var currentPath = "Assets";
            foreach (string folder in folders) {
                if (string.IsNullOrEmpty(folder))
                    continue;
                string newPath = string.Format("{0}/{1}", currentPath, folder);
                if (!AssetDatabase.IsValidFolder(newPath))
                    AssetDatabase.CreateFolder(currentPath, folder);
                currentPath = newPath;
            }
        }
    }
}
