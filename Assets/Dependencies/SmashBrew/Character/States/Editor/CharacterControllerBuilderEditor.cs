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
            var controllerProperty = serializedObject.FindProperty("_animatorController");
            EditorGUILayout.PropertyField(controllerProperty);
            filter = EditorGUILayout.TextField("Filter", filter);
            var text = controllerProperty.objectReferenceValue != null ? "Update Animator Controller" : "Create Animation Controller";
            if (GUILayout.Button(text)) {
                var controller = controllerProperty.objectReferenceValue as AnimatorController;
                Log.Debug(controller);
                if (controller == null)
                    controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/" + target.GetType().Name + ".controller");
                var stateMachine = controller.layers[0].stateMachine;
                var builder = target as CharacterControllerBuilder;
                foreach(var state in stateMachine.states)
                    stateMachine.RemoveState(state.state);
                foreach (var state in builder.Builder.States) {
                    var animatorState = stateMachine.AddState(state.AnimatorName);
                    var clip = state.Data.AnimationClip;
                    animatorState.motion = clip;
                    if (clip == null || state.Data.Length <= 0)
                        animatorState.speed = 1f;
                    else {
                        Log.Debug(clip);
                        animatorState.speed = clip.length / state.Data.Length;
                    }
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
                var elementData = element.FindPropertyRelative("Data");
                var clip = elementData.FindPropertyRelative("AnimationClip");
                var length = elementData.FindPropertyRelative("Length");
                var animationClip = clip.objectReferenceValue as AnimationClip;
                EditorGUILayout.PropertyField(
                    elementData,
                    new GUIContent(name),
                    true
                );
                var newClip = clip.objectReferenceValue as AnimationClip;
                if (animationClip != newClip && newClip != null)
                    length.floatValue = newClip.length;
            }
            if (GUI.changed)
                serializedObject.ApplyModifiedProperties();
        }

    }

}
