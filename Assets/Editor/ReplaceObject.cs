#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace GOReplacer{
	public class GameObjectReplacer : EditorWindow {
	
		private GameObject[] gameObjs;
		private GameObject gameObjReplacement;
		private bool _canReplace, _useRotation, _useScale;
		private string replaceButton, replaceDialog;
		
		[MenuItem ("GameObject/Replace GameObject")]
		static void Init () {
			#pragma warning disable
			GameObjectReplacer window = (GameObjectReplacer)EditorWindow.GetWindow (typeof (GameObjectReplacer));
			#pragma warning restore
		}
		
		void OnGUI () {
			gameObjReplacement = (GameObject)EditorGUILayout.ObjectField("Replacement",gameObjReplacement, typeof(GameObject),true);
			_useRotation = EditorGUILayout.Toggle("Use Selected Rotation", _useRotation);
			_useScale = EditorGUILayout.Toggle("Use Selected Scale", _useScale);
			if(GUILayout.Button(replaceButton)){
				if(gameObjs.Length != 0 && gameObjReplacement){
					if(EditorUtility.DisplayDialog("Are you sure?",replaceDialog + "\n\nThere's no going back once you hit OK!","OK","Cancel")){
						if(!PrefabUtility.GetPrefabObject(gameObjReplacement)){
							if(EditorUtility.DisplayDialog("Error!","Your replacement object must be a prefab!","OK")){
								return;
							}
						} else {
							_canReplace = true;
						}
					}
				} else {
					EditorUtility.DisplayDialog("Missing GameObjects!", "Make sure you have both a Replacement GameObject and have selected 1 or more GameObjects in the scene.", "OK");
				}
			}
		}
		
		void Update(){
			gameObjs = Selection.gameObjects;
			if(_canReplace){
				Replace();
			}
			if(gameObjs.Length > 1){
				replaceButton = "Replace GameObjects";
				replaceDialog = "Are you sure you want to replace these GameObjects?";
			} else {
				replaceButton = "Replace GameObject";
				replaceDialog = "Are you sure you want to replace this GameObject?";
			}
		}
		
		private void Replace(){
			foreach(GameObject gameObj in gameObjs){
				GameObject newGameObj = PrefabUtility.InstantiatePrefab(gameObjReplacement) as GameObject;
				newGameObj.transform.position = gameObj.transform.position;
				if(_useRotation){
					newGameObj.transform.rotation = gameObj.transform.rotation;
				}
				if(_useScale){
					newGameObj.transform.localScale = gameObj.transform.localScale;
				}
				newGameObj.transform.parent = gameObj.transform.parent;
				DestroyImmediate(gameObj);
			}
			_canReplace = false;
		}
	}
}
#endif