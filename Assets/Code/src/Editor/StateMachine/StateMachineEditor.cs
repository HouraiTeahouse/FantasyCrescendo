using System.IO;
using UnityEngine;
using UnityEditor;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

[CustomEditor(typeof(StateMachineAsset))]
public class StateMachineEditor : Editor {
    
    public override void OnInspectorGUI() {
        if (GUILayout.Button("Open State Machine Editor")) {
            ShowGraphEditWindow(AssetDatabase.GetAssetPath(target));
        }
    }

    static bool ShowGraphEditWindow(string path) {
        var guid = AssetDatabase.AssetPathToGUID(path);

        var foundWindow = false;
        foreach (var w in Resources.FindObjectsOfTypeAll<TestEditorWindow>()) {
            if (w.SelectedGuid == guid) {
                foundWindow = true;
                w.Focus();
            }
        }

        if (!foundWindow)
        {
            var window = CreateInstance<TestEditorWindow>();
            window.Show();
            window.Initialize(guid);
        }

        return true;
    }

}

}
