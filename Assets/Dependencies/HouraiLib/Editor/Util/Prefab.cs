using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace HouraiTeahouse.Editor {

    public static class Prefab {

        public static bool IsPrefab(this UnityObject obj) {
            if (obj == null)
                return false;
            switch (PrefabUtility.GetPrefabType(obj)) {
                case PrefabType.Prefab:
                case PrefabType.ModelPrefab:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsPrefabInstance(this UnityObject obj) {
            if (obj == null)
                return false;
            switch (PrefabUtility.GetPrefabType(obj)) {
                case PrefabType.None:
                case PrefabType.Prefab:
                case PrefabType.ModelPrefab:
                    return false;
                default:
                    return true;
            }
        }

        public static UnityObject GetPrefab(this UnityObject obj) {
            if (obj == null)
                return null;

            UnityObject prefab;

            switch (PrefabUtility.GetPrefabType(obj)) {
                case PrefabType.Prefab:
                case PrefabType.ModelPrefab:
                    return obj;
                default:
                    prefab = PrefabUtility.GetPrefabParent(obj);
                    break;
            }
            return prefab;
        }

        public static UnityObject CreatePrefab(string folderPath,
                                               GameObject obj = null,
                                               ReplacePrefabOptions options = ReplacePrefabOptions.ConnectToPrefab) {
            //Create Folder if it doesn't already exist
            Assets.CreateFolder(folderPath);

            folderPath = "Assets/" + folderPath;

            if (obj == null)
                return PrefabUtility.CreateEmptyPrefab(folderPath + "New Prefab.prefab");
            return PrefabUtility.CreatePrefab(folderPath + "/" + obj.name + ".prefab", obj, options);
        }

    }

}