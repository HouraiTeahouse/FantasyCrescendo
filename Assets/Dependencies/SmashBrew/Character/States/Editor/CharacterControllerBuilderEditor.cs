using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.Timeline;
using UnityEngine.Playables;
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

        AnimationTrack GetOrCreateAnimationTrack(string name, TimelineAsset timeline, Action<AnimationTrack> prepareFunc = null) {
            var track = timeline.GetRootTracks().OfType<AnimationTrack>()
                                                .FirstOrDefault(t => t != null && t.name == name);
            if (track != null) {
                prepareFunc.SafeInvoke(track);
            } else {
                track = timeline.CreateTrack<AnimationTrack>(null, name);
            }
            return track;
        }

        public override void OnInspectorGUI() {
            var prefabProperty = serializedObject.FindProperty("_prefab");
            EditorGUILayout.PropertyField(prefabProperty);
            filter = EditorGUILayout.TextField("Filter", filter);
            var text = prefabProperty.objectReferenceValue != null ? "Update Animator Controller" : "Create Animation Controller";
            if (GUILayout.Button(text)) {
                var directory = Path.GetDirectoryName(AssetDatabase.GetAssetPath(target));
                var builder = target as CharacterControllerBuilder;
                var prefab = prefabProperty.objectReferenceValue as GameObject;
                var director = prefab == null ? null : prefab.GetComponentInChildren<PlayableDirector>();
                var animator = prefab == null ? null : prefab.GetComponentInChildren<Animator>();
                var animatorGo = animator == null ? null : animator.gameObject;
                foreach (var state in builder.Builder.States) {
                    TimelineAsset timeline = state.Data.Timeline;
                    if (state.Data.Timeline == null) {
                        timeline = ScriptableObject.CreateInstance<TimelineAsset>();
                        AssetDatabase.CreateAsset(timeline, Path.Combine(directory, builder.name + "_" + state.Name + ".playable"));
                        state.Data.Timeline = timeline;
                    }
                    if (state.Data.AnimationClip == null)
                        continue;
                    var baseAnimationTrack = GetOrCreateAnimationTrack("base_animation", timeline, track => {
                        foreach (var timelineClip in track.GetClips())
                            timeline.DeleteClip(timelineClip);
                    });
                    var hitboxAnimationTrack = GetOrCreateAnimationTrack("hitbox_animation", timeline);

                    // Create base animation clip
                    var baseClip = baseAnimationTrack.CreateClip(state.Data.AnimationClip);
                    baseClip.displayName = state.Data.AnimationClip.name + "_Animation";

                    if (director != null && animator != null) {
                        director.SetGenericBinding(baseAnimationTrack, animatorGo);
                        director.SetGenericBinding(hitboxAnimationTrack, animatorGo);
                    }

                    EditorUtility.SetDirty(timeline);
                    EditorUtility.SetDirty(baseAnimationTrack);
                    EditorUtility.SetDirty(hitboxAnimationTrack);
                }
                EditorUtility.SetDirty(prefab);
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
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
