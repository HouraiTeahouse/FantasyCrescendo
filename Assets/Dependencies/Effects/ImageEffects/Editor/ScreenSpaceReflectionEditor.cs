using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects {

    [CustomEditor(typeof(ScreenSpaceReflection))]
    internal class ScreenSpaceReflectionEditor : Editor {

        [Serializable]
        class CatFoldoutMap {

            public ScreenSpaceReflection.SSRSettings.LayoutAttribute.Category category;
            public bool display;

            public CatFoldoutMap(ScreenSpaceReflection.SSRSettings.LayoutAttribute.Category category, bool display) {
                this.category = category;
                this.display = display;
            }

        }

        enum SettingsMode {

            HighQuality,
            Default,
            Performance,
            Custom,

        }

        static class StaticFieldFinder<T> {

            public static FieldInfo GetField<TValue>(Expression<Func<T, TValue>> selector) {
                Expression body = selector;
                if (body is LambdaExpression) {
                    body = ((LambdaExpression) body).Body;
                }
                switch (body.NodeType) {
                    case ExpressionType.MemberAccess:
                        return (FieldInfo) ((MemberExpression) body).Member;
                    default:
                        throw new InvalidOperationException();
                }
            }

        }

        class Styles {

            public readonly GUIStyle header = "ShurikenModuleTitle";

            internal Styles() {
                header.font = new GUIStyle("Label").font;
                header.border = new RectOffset(15, 7, 4, 4);
                header.fixedHeight = 22;
                header.contentOffset = new Vector2(20f, -2f);
            }

        }

        static Styles m_Styles;

        readonly
            Dictionary<FieldInfo, KeyValuePair<ScreenSpaceReflection.SSRSettings.LayoutAttribute, SerializedProperty>>
            m_PropertyMap =
                new Dictionary
                    <FieldInfo, KeyValuePair<ScreenSpaceReflection.SSRSettings.LayoutAttribute, SerializedProperty>>();

        [SerializeField]
        List<CatFoldoutMap> m_CategoriesToShow = new List<CatFoldoutMap>();

        [NonSerialized]
        bool m_Initialized;

        void PopulateMap(FieldInfo prefix, FieldInfo field) {
            string searchPath = prefix.Name + "." + field.Name;

            var attr =
                field.GetCustomAttributes(typeof(ScreenSpaceReflection.SSRSettings.LayoutAttribute), false)
                    .FirstOrDefault() as ScreenSpaceReflection.SSRSettings.LayoutAttribute;
            if (attr == null)
                attr =
                    new ScreenSpaceReflection.SSRSettings.LayoutAttribute(
                        ScreenSpaceReflection.SSRSettings.LayoutAttribute.Category.Undefined,
                        0);

            m_PropertyMap.Add(field,
                new KeyValuePair<ScreenSpaceReflection.SSRSettings.LayoutAttribute, SerializedProperty>(attr,
                    serializedObject.FindProperty(searchPath)));
        }

        void Initialize() {
            m_Styles = new Styles();
            IEnumerable<ScreenSpaceReflection.SSRSettings.LayoutAttribute.Category> categories =
                Enum.GetValues(typeof(ScreenSpaceReflection.SSRSettings.LayoutAttribute.Category))
                    .Cast<ScreenSpaceReflection.SSRSettings.LayoutAttribute.Category>();
            foreach (ScreenSpaceReflection.SSRSettings.LayoutAttribute.Category cat in categories) {
                if (m_CategoriesToShow.Any(x => x.category == cat))
                    continue;

                m_CategoriesToShow.Add(new CatFoldoutMap(cat, true));
            }

            FieldInfo prefix = StaticFieldFinder<ScreenSpaceReflection>.GetField(x => x.settings);
            foreach (
                FieldInfo field in
                    typeof(ScreenSpaceReflection.SSRSettings).GetFields(BindingFlags.Public | BindingFlags.Instance))
                PopulateMap(prefix, field);

            m_Initialized = true;
        }

        public override void OnInspectorGUI() {
            if (!m_Initialized)
                Initialize();

            ScreenSpaceReflection.SSRSettings currentState = ((ScreenSpaceReflection) target).settings;

            var settingsMode = SettingsMode.Custom;
            if (currentState.Equals(ScreenSpaceReflection.SSRSettings.performanceSettings))
                settingsMode = SettingsMode.Performance;
            else if (currentState.Equals(ScreenSpaceReflection.SSRSettings.defaultSettings))
                settingsMode = SettingsMode.Default;
            else if (currentState.Equals(ScreenSpaceReflection.SSRSettings.highQualitySettings))
                settingsMode = SettingsMode.HighQuality;

            EditorGUI.BeginChangeCheck();
            settingsMode = (SettingsMode) EditorGUILayout.EnumPopup("Preset", settingsMode);
            if (EditorGUI.EndChangeCheck())
                Apply(settingsMode);

            DrawFields();
            serializedObject.ApplyModifiedProperties();
        }

        IEnumerable<SerializedProperty> GetProperties(
            ScreenSpaceReflection.SSRSettings.LayoutAttribute.Category category) {
            return
                m_PropertyMap.Values.Where(x => x.Key.category == category)
                    .OrderBy(x => x.Key.priority)
                    .Select(x => x.Value);
        }

        bool Header(string title, bool display) {
            Rect rect = GUILayoutUtility.GetRect(16f, 22f, m_Styles.header);
            GUI.Box(rect, title, m_Styles.header);

            var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            if (Event.current.type == EventType.Repaint)
                EditorStyles.foldout.Draw(toggleRect, false, false, display, false);

            Event e = Event.current;
            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition)) {
                display = !display;
                e.Use();
            }
            return display;
        }

        void DrawFields() {
            foreach (CatFoldoutMap cat in m_CategoriesToShow) {
                IEnumerable<SerializedProperty> properties = GetProperties(cat.category);
                if (!properties.Any())
                    continue;

                GUILayout.Space(5);
                cat.display = Header(cat.category.ToString(), cat.display);

                if (!cat.display)
                    continue;

                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.BeginVertical();
                GUILayout.Space(3);
                foreach (SerializedProperty field in GetProperties(cat.category))
                    EditorGUILayout.PropertyField(field);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }

        void Apply(SettingsMode settingsMode) {
            switch (settingsMode) {
                case SettingsMode.Default:
                    Apply(ScreenSpaceReflection.SSRSettings.defaultSettings);
                    break;
                case SettingsMode.HighQuality:
                    Apply(ScreenSpaceReflection.SSRSettings.highQualitySettings);
                    break;
                case SettingsMode.Performance:
                    Apply(ScreenSpaceReflection.SSRSettings.performanceSettings);
                    break;
            }
        }

        void Apply(ScreenSpaceReflection.SSRSettings settings) {
            foreach (
                KeyValuePair
                    <FieldInfo, KeyValuePair<ScreenSpaceReflection.SSRSettings.LayoutAttribute, SerializedProperty>>
                    fieldKVP in m_PropertyMap) {
                object value = fieldKVP.Key.GetValue(settings);
                Type fieldType = fieldKVP.Key.FieldType;

                if (fieldType == typeof(float)) {
                    fieldKVP.Value.Value.floatValue = (float) value;
                }
                else if (fieldType == typeof(bool)) {
                    fieldKVP.Value.Value.boolValue = (bool) value;
                }
                else if (fieldType == typeof(int)) {
                    fieldKVP.Value.Value.intValue = (int) value;
                }
                else if (fieldType.IsEnum) {
                    fieldKVP.Value.Value.enumValueIndex = (int) value;
                }
                else {
                    Debug.LogErrorFormat("Encounted unexpected type {0} in application of settings",
                        fieldKVP.Key.FieldType);
                }
            }
        }

    }

}