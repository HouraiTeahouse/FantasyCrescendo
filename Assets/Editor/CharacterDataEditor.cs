using UnityEngine;
using System.Collections;
using UnityEditor;

public class CharacterDataEditor : EditorWindow {

	

	[MenuItem("Window/Character Editor")]
	public static void Init() {
		EditorWindow.GetWindow<CharacterDataEditor> ("Character");
	}

	void OnGUI() {

	}
}
