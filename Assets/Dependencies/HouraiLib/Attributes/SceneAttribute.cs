// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse {

    /// <summary> PropertyAttribute with a drawer that exposes a SceneAsset object field. MUST be a string field. Saves the
    /// path of the SceneAsset to the field. </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SceneAttribute : PropertyAttribute {
    }

#if UNITY_EDITOR
    /// <summary> Custom PropertyDrawer for SceneAttribute </summary>
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    internal class SceneAttributeDrawer : PropertyDrawer {

        readonly Dictionary<SerializedProperty, SceneAsset> _scenes;

        public SceneAttributeDrawer() {
            _scenes = new Dictionary<SerializedProperty, SceneAsset>();
        }

        /// <summary>
        ///     <see cref="PropertyDrawer.OnGUI" />
        /// </summary>
        public override void OnGUI(Rect position,
                                   SerializedProperty property,
                                   GUIContent label) {
            if (property.propertyType != SerializedPropertyType.String) {
                base.OnGUI(position, property, label);
                return;
            }
            if (!_scenes.ContainsKey(property))
                _scenes[property] =
                    AssetDatabase.LoadAssetAtPath<SceneAsset>(
                        "Assets/{0}.unity".With(property.stringValue));
            EditorGUI.BeginChangeCheck();
            _scenes[property] =
                EditorGUI.ObjectField(position,
                    label,
                    _scenes[property],
                    typeof(SceneAsset),
                    false) as SceneAsset;
            if (EditorGUI.EndChangeCheck())
                property.stringValue =
                    Regex.Replace(
                        AssetDatabase.GetAssetPath(_scenes[property]),
                        "Assets/(.*)\\.unity",
                        "$1");
        }

    }
#endif

}
