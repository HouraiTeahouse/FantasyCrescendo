using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.Editor {

    /// <summary> Utility class for handling Assets in the Unity Editor. </summary>
    [InitializeOnLoad]
    public static class Assets {

        const string ResourcePath = "Resources/";
        static readonly Regex ResourceRegex = new Regex(".*/Resources/(.*?)\\..*", RegexOptions.Compiled);

        /// <summary> Create new asset from <see cref="ScriptableObject" /> type with unique name at selected folder in project
        /// window. Asset creation can be cancelled by pressing escape key when asset is initially being named. </summary>
        /// <typeparam name="T"> type of scriptable object </typeparam>
        public static T CreateAssetInProjectWindow<T>(T asset = null, string name = null) where T : ScriptableObject {
            if (asset == null)
                asset = ScriptableObject.CreateInstance<T>();
            if (name.IsNullOrEmpty())
                name = "New {0}.asset".With(typeof(T).Name);
            ProjectWindowUtil.CreateAsset(asset, name);
            return asset;
        }

        public static bool IsAsset(this Object obj) {
            return AssetDatabase.IsMainAsset(obj) || AssetDatabase.IsSubAsset(obj) || AssetDatabase.IsForeignAsset(obj)
                || AssetDatabase.IsNativeAsset(obj);
        }

        public static void CreateAsset(string folder, Object obj, string suffix = null) {
            if (obj.IsAsset())
                return;
            Check.NotNull(folder);
            Check.NotNull(obj);
            suffix = suffix ?? "asset";
            // Create folder if it doesn't already exist
            CreateFolder(folder);
            AssetDatabase.CreateAsset(obj, string.Format("{0}/{1}.{2}", folder, obj.name, suffix));
        }

        public static IEnumerable<string> GetAssetGUIDs<T>(string nameFilter = null) where T : Object {
            string format = string.IsNullOrEmpty(nameFilter) ? "t:{0}" : "t:{0} {1}";
            return from guid in AssetDatabase.FindAssets(string.Format(format, typeof(T).Name, nameFilter)) select guid;
        }

        public static IEnumerable<string> GetAssetPaths<T>(string nameFilter = null) where T : Object {
            return from guid in GetAssetGUIDs<T>() select AssetDatabase.GUIDToAssetPath(guid);
        }

        public static T LoadOrDefault<T>(string nameFilter = null) where T : Object {
            return AssetDatabase.LoadAssetAtPath<T>(GetAssetPaths<T>(nameFilter).FirstOrDefault());
        }

        public static T LoadOrCreate<T>(string nameFilter = null) where T : ScriptableObject {
            var loaded = LoadOrDefault<T>();
            return loaded ?? CreateAssetInProjectWindow<T>();
        }

        public static bool IsResourcePath(string path) {
            return !string.IsNullOrEmpty(path) && path.Contains(ResourcePath);
        }

        public static bool IsResource(Object asset) { return IsResourcePath(AssetDatabase.GetAssetPath(asset)); }

        public static string GetResourcePath(Object asset) {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            if (!IsResourcePath(assetPath))
                return string.Empty;
            return ResourceRegex.Replace(assetPath, "$1");
        }

        public static void CreateFolder(string path) {
            if (AssetDatabase.IsValidFolder(path))
                return;
            string[] folders = path.Split('/');
            string currentPath = string.Empty;
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