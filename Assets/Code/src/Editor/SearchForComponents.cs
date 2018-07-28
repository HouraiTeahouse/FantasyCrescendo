using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
 
public class SearchForComponents : EditorWindow {

  [MenuItem("Hourai Teahouse/Search For Components")]
  static void Init () => EditorWindow.GetWindow<SearchForComponents>();

  string[] modes = new string[] { 
    "Search for component usage", 
    "Search for missing components" 
  };
 
   List<string> listResult;
   int editorMode, editorModeOld;
   MonoScript targetComponent;
   string componentName = "";
   Vector2 scroll;

   void SelectorBar() {
     GUILayout.Space( 3 );
     int oldValue = GUI.skin.window.padding.bottom;
     GUI.skin.window.padding.bottom = -20;
     Rect windowRect = GUILayoutUtility.GetRect( 1, 17 );
     windowRect.x += 4;
     windowRect.width -= 7;
     editorMode = GUI.SelectionGrid( windowRect, editorMode, modes, 2, "Window" );
     GUI.skin.window.padding.bottom = oldValue;
 
     if ( editorModeOld != editorMode ) {
       editorModeOld = editorMode;
       listResult?.Clear();
       componentName = targetComponent == null ? "" : targetComponent.name;
     }
   }

   void FindScriptsOfType() {
    targetComponent = (MonoScript) EditorGUILayout.ObjectField( targetComponent, typeof( MonoScript ), false );

    if (!GUILayout.Button("Search")) return;
    componentName = targetComponent.name;
    AssetDatabase.SaveAssets();
    string targetPath = AssetDatabase.GetAssetPath( targetComponent );
    listResult = (from prefab in GetAllPrefabs()
                  from dep in AssetDatabase.GetDependencies(prefab)
                  where dep == targetPath
                  select dep).ToList();
   }

   void FindMissingScripts() {
      if (!GUILayout.Button("Search")) return;
      var prefabs = GetAllPrefabs().Select(path => AssetDatabase.LoadMainAssetAtPath(path) as GameObject)
                                    .Where(go => go != null);
      listResult = (from gameObj in prefabs
                    from comp in gameObj.GetComponentsInChildren<Component>(true)
                    where comp == null
                    select AssetDatabase.GetAssetPath(comp)).ToList();
   }

   void DisplayResults() {
     if (listResult == null) return;
     if ( listResult?.Count == 0 ) {
       GUILayout.Label( editorMode == 0 ? ( componentName == "" ? "Choose a component" : "No prefabs use component " + componentName ) : ( "No prefabs have missing components!\nClick Search to check again" ) );
     } else {
       GUILayout.Label( editorMode == 0 ? ( "The following prefabs use component " + componentName + ":" ) : ( "The following prefabs have missing components:" ) );
       scroll = GUILayout.BeginScrollView( scroll );
       foreach (string path in listResult) {
         GUILayout.BeginHorizontal();
         GUILayout.Label(path, GUILayout.Width(position.width / 2));
         if (GUILayout.Button("Select", GUILayout.Width(position.width / 2 - 10))) {
           Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
         }
         GUILayout.EndHorizontal();
       }
       GUILayout.EndScrollView();
     }
   }
 
   void OnGUI () {
     SelectorBar();
     switch (editorMode) {
       case 0: FindScriptsOfType(); break;
       case 1: FindMissingScripts(); break;
     }
     DisplayResults();
   }
 
   static IEnumerable<string> GetAllPrefabs () => 
      from path in (
        from guid in AssetDatabase.FindAssets("t:GameObject")
        select AssetDatabase.GUIDToAssetPath(guid)
      )
      where path.EndsWith(".prefab")
      select path;

}