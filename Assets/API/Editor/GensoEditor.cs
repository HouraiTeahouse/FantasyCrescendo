using UnityEngine;
using UnityEditor;

namespace Genso.API.Editor {

    public class GensoEditor
    {

        [MenuItem("Assets/Create/Game Config")]
        public static void CreateConfig()
        {
            CreateAsset<Config>();
        }

        [MenuItem("Assets/Create/Character Data")]
        public static void CreateCharacterData() {
            CreateAsset<CharacterData>();
        }

        [MenuItem("Tools/Clear Player Prefs %#c")]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Player Prefs Cleared.");
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