using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class GensoEditor {

    static GensoEditor() {
#if UNITY_EDITOR
        var settings = Resources.Load<GameSettings>("GameSettings");
        if (settings == null) {
            settings = ScriptableObject.CreateInstance<GameSettings>();
            AssetDatabase.CreateAsset(settings, "Assets/Resources/GameSettings.asset");
        }
#endif
    }

}
