using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Crescendo.API {

    public static class AssetUtil {

        public static string CreateAssetPath(params string[] folderNames) {
            if (folderNames == null)
                throw new ArgumentNullException("folderNames");

            if (folderNames.Length <= 0)
                return "";

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
            if (folder == null)
                throw new ArgumentNullException("folder");
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (suffix == null)
                suffix = "asset";

            if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(obj)))
                return;

            // Create folder if it doesn't already exist
            CreateFolder(folder);

            AssetDatabase.CreateAsset(obj, "Assets/" + folder + "/" + obj.name + "." + suffix);
        }

        public static string[] FindAssetPaths<T>() where T : Object {
            List<string> paths = new List<string>();
            foreach (var guid in AssetDatabase.FindAssets("t:" + typeof(T).Name))
                paths.Add(AssetDatabase.GUIDToAssetPath(guid));
            return paths.ToArray();
        }

        public static void CreateFolder(string path) {
            if (path == null)
                throw new ArgumentNullException("path");

            if (IsValidFolder(path))
                return;

            string[] folders = path.Split('/');
            var currentPath = "Assets";
            for (var i = 0; i < folders.Length; i++) {
                if (string.IsNullOrEmpty(folders[i]))
                    continue;

                string newPath = currentPath + "/" + folders[i];
                if (!AssetDatabase.IsValidFolder(currentPath + "/" + folders[i]))
                    AssetDatabase.CreateFolder(currentPath, folders[i]);
                currentPath = newPath;
            }
        }


    }

}
