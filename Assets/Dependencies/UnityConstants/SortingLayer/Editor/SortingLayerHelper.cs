using UnityEditor;
using System;
using System.Linq;
using System.Reflection;

namespace HouraiTeahouse {
    // Helpers used by the different sorting layer classes.
    [InitializeOnLoad]
    public static class SortingLayerHelper {
        static readonly PropertyInfo SortingLayerNamesProperty;
        static readonly MethodInfo GetSortingLayerIdMethod;

        static SortingLayerHelper() {
            Type utilityType = Type.GetType("UnityEditorInternal.InternalEditorUtility, UnityEditor");
            SortingLayerNamesProperty = utilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);

            // Unity 5.0 calls this "GetSortingLayerUniqueID" but in 4.x it was "GetSortingLayerUserID".
            GetSortingLayerIdMethod = utilityType.GetMethod("GetSortingLayerUserID",
                BindingFlags.Static | BindingFlags.NonPublic);
        }

        // Gets an array of sorting layer names.
        // Since this uses reflection, callers should check for 'null' which will be returned if the reflection fails.
        public static string[] SortingLayerNames {
            get { return SortingLayerNamesProperty.GetValue(null, null) as string[]; }
        }

        // Given the ID of a sorting layer, returns the sorting layer's name
        public static string GetSortingLayerNameFromID(int id) {
            string[] names = SortingLayerNames;
            if (names == null)
                return null;
            return names.Where((t, i) => GetSortingLayerIDForIndex(i) == id).FirstOrDefault();
        }

        // Given the name of a sorting layer, returns the ID.
        public static int GetSortingLayerIDForName(string name) {
            string[] names = SortingLayerNames;
            if (names == null)
                return 0;
            return GetSortingLayerIDForIndex(Array.IndexOf(names, name));
        }

        // Helper to convert from a sorting layer INDEX to a sorting layer ID. These are not the same thing.
        // IDs are based on the order in which layers were created and do not change when reordering the layers.
        // Thankfully there is a helper we can call to get the ID for a layer given its index.
        public static int GetSortingLayerIDForIndex(int index) {
            if (GetSortingLayerIdMethod == null)
                return 0;
            return (int)GetSortingLayerIdMethod.Invoke(null, new object[] { index });
        }
    }
}
