using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using HouraiTeahouse.SmashBrew.States;

namespace HouraiTeahouse.SmashBrew.Characters {

    [CustomEditor(typeof(CharacterControllerBuilder), isFallback = true)]
    public class CharacterControllerBuilderEditor : UnityEditor.Editor {

        string filter = string.Empty;

        void OnEnable() {
            var builder = target as CharacterControllerBuilder;
            builder.BuildCharacterControllerImpl(new StateControllerBuilder<CharacterState, CharacterStateContext>());
            EditorUtility.SetDirty(builder);
        }

        public override void OnInspectorGUI() {
            filter = EditorGUILayout.TextField("Filter", filter);
            if (GUILayout.Button("Create Animation Controller")) {
                var controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/" + target.GetType().Name + ".controller");
                var stateMachine = controller.layers[0].stateMachine;
                var builder = target as CharacterControllerBuilder;
                foreach (var state in builder.Builder.States) {
                    var animState = stateMachine.AddState(state.AnimatorName);
                }
                const int x = 205;
                const int y = 45;
                const int colSize = 15;
                var states = stateMachine.states;
                for (var i = 0; i < states.Length; i++) {
                    states[i].position = new Vector2((i/ colSize) * x, (i % colSize) * y);
                }
                stateMachine.states = states;
            }
            EditorGUILayout.Space();
            var data = serializedObject.FindProperty("_data");
            for (var i = 0; i < data.arraySize; i++) {
                var element = data.GetArrayElementAtIndex(i);
                var name = element.FindPropertyRelative("Name").stringValue;
                if (!string.IsNullOrEmpty(filter) && !name.ToUpper().Contains(filter.ToUpper()))
                    continue;
                EditorGUILayout.PropertyField(
                    element.FindPropertyRelative("Data"),
                    new GUIContent(name),
                    true
                );
            }
            if (GUI.changed)
                serializedObject.ApplyModifiedProperties();
        }

    }

}
