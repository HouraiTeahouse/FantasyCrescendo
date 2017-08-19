using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEditor;

namespace HouraiTeahouse {

    public class EditorTable<T> {

        [InitializeOnLoad]
        public class Column {

            public string Name { get; set; }
            public float Width { get; set; }
            public Action<Rect, T> _drawFunc;

            public Column(Action<Rect, T> drawFunc) {
                _drawFunc = drawFunc;
            }

            internal void Draw(Rect rect, T obj)  {
                _drawFunc.SafeInvoke(rect, obj);
            }

        }

        public float RowHeight { get; set; }
        public float LabelPadding { get; set; }
        public Vector2 Padding { get; set; }
        public ReadOnlyCollection<Column> Columns { get; private set; }
        public GUIStyle LabelStyle { get; set; }
        List<Column> _columns;
        Vector2 _scrollPos;

        public EditorTable() {
            RowHeight = 16f;
            _columns = new List<Column>();
            Columns = new ReadOnlyCollection<Column>(_columns);
        }

        public Column AddColumn(Action<Rect, T> drawFunc = null) {
            var col = new Column(drawFunc);
            _columns.Add(col);
            return col;
       }

        public void Draw(Rect area, IEnumerable<T> set) {
            var rect = new Rect(area.x, area.y, 0, RowHeight);
            var rowRect = rect;
            rowRect.width = area.width;
            if (LabelStyle != null)
                GUI.Box(rowRect, string.Empty, LabelStyle);
            DrawRow(ref rect, area.width, (col, colRect) => {
                EditorGUI.LabelField(colRect, col.Name);
            });
            rect.y += LabelPadding;
            foreach (var element in set) {
                Log.Debug((element as SerializedObject).targetObject.name);
                var isUnityObject = element is UnityEngine.Object;
                EditorGUI.BeginChangeCheck();
                DrawRow(ref rect, area.width, (col, colRect) => col.Draw(colRect, element));
                if (!EditorGUI.EndChangeCheck())
                    continue;
                if (isUnityObject) {
                    EditorUtility.SetDirty(element as UnityEngine.Object);
                } else {
                    var serializedProperty = element as SerializedProperty;
                    var serializedObject = element as SerializedObject;
                    if (serializedObject == null && serializedProperty != null)
                        serializedObject = serializedProperty.serializedObject;
                    if (serializedObject != null)
                        serializedObject.ApplyModifiedProperties();
                }
            }
        }

        void DrawRow(ref Rect position, float width, Action<Column, Rect> colFunc) {
            var xPos = position.x;
            foreach (var column in Columns) {
                position.width = width * column.Width;
                colFunc.SafeInvoke(column, position);
                position.x += position.width + Padding.x;
            }
            position.x = xPos;
            position.y += RowHeight + Padding.y;
        }

    }

    public static class EditorTableExtensions {

        public static EditorTable<SerializedObject>.Column AddPropertyColumn(this EditorTable<SerializedObject> table, 
                                                                             string propertyName) {
            return Argument.NotNull(table).AddColumn((rect, obj) => {
                EditorGUI.PropertyField(rect, obj.FindProperty(propertyName), GUIContent.none, false);
            });
        }

        public static EditorTable<SerializedProperty>.Column AddPropertyColumn(this EditorTable<SerializedProperty> table, 
                                                                             string propertyName) {
            return Argument.NotNull(table).AddColumn((rect, obj) => {
                EditorGUI.PropertyField(rect, obj.FindPropertyRelative(propertyName), GUIContent.none, false);
            });
        }

    }

}
