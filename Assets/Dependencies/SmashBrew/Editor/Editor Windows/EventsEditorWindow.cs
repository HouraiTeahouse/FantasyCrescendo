using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HouraiTeahouse.Editor;
using UnityEditor;
using UnityEditor.Animations;

namespace HouraiTeahouse.SmashBrew.Editor {

    public class EventsEditorWindow : LockableEditorWindow {

        public AnimatorState State { get; set; }
        float SeekTime { get; set; }
        Hitbox.Type _filter;
        ObjectSelector<CharacterData> _characters;
        Hitbox[] _hitboxes;
        string error;

        static EventsEditorWindow GetWindow() {
            return GetWindow<EventsEditorWindow>("Events");
        }

        [MenuItem("Window/Animator Events Editor")]
        static void CreateWindow() {
            EventsEditorWindow window = GetWindow();
            window.Show();
            window.Refresh();
        }

        [MenuItem("Smash Brew/Add Offensive Hitbox %h")]
        static void AddOffensiveHitbox() {
            AddHitbox(Hitbox.Type.Offensive); 
        }

        [MenuItem("Smash Brew/Add Hurtbox %#h")]
        static void AddHurtbox() {
            AddHitbox(Hitbox.Type.Damageable);
        }

        [MenuItem("Smash Brew/Add Hurtbox %#h", true)]
        [MenuItem("Smash Brew/Add Offensive Hitbox %h", true)]
        static bool AddHitboxValidate() {
            return Selection.gameObjects.Length > 0;
        }

        static void AddHitbox(Hitbox.Type type) {
            var hitboxes = new List<Hitbox>();
            var rootMap = new Dictionary<GameObject, List<Hitbox>>();
            var idGen = new System.Random();
            Undo.IncrementCurrentGroup();
            foreach (GameObject go in Selection.gameObjects) {
                var hbGo = new GameObject();
                Undo.RegisterCreatedObjectUndo(hbGo, "Create Hitbox GameObject");
                var collider = Undo.AddComponent<SphereCollider>(hbGo);
                var hb = Undo.AddComponent<Hitbox>(hbGo);
                hb.CurrentType = type;
                hb.ID = idGen.Next();
                hitboxes.Add(hb);
                Undo.SetTransformParent(hb.transform, go.transform, "Parent Hitbox");
                hb.transform.Reset();
                Undo.RecordObject(collider, "Edit Collider Size");
                collider.radius = 1f /
                    ((Vector3) (hb.transform.localToWorldMatrix * Vector3.one))
                        .Max();
                var character = hbGo.GetComponentInParent<Character>();
                GameObject rootGo = character != null
                    ? character.gameObject
                    : hb.transform.root.gameObject;
                if(!rootMap.ContainsKey(rootGo))
                    rootMap[rootGo] = new List<Hitbox>();
                rootMap[rootGo].Add(hb);
            }
            foreach (var set in rootMap) {
                Hitbox[] allHitboxes = set.Key.GetComponentsInChildren<Hitbox>();
                int i = allHitboxes.Length - set.Value.Count;
                Undo.RecordObjects(set.Value.ToArray(), "Name Changes");
                foreach (Hitbox hitbox in set.Value) {
                    hitbox.name = string.Format("{0}_hb_{1}_{2}", set.Key.name, type, i).ToLower();
                    i++;
                }
            }
            Selection.objects = hitboxes.GetGameObject().ToArray();
            Undo.SetCurrentGroupName(string.Format("Generate {0} Hitbox{1}", type, hitboxes.Count > 0 ? "es" : string.Empty));
            GetWindow().Refresh();
        }

        // Loads all of the Characters from Resources.
        static CharacterData[] LoadAllCharacters() {
            return Resources.LoadAll<CharacterData>(string.Empty);
        }

        static Hitbox[] GetHitboxes(CharacterData character) {
            if (character == null || character.Prefab.Load() == null)
                return null;
            return character.Prefab.Load().GetComponentsInChildren<Hitbox>();
        }

        static CharacterData FindBestHitboxMatch(IEnumerable<int> ids,
                                                 CharacterData[] characters) {
            if (characters == null)
                return null;
            var idSet = new HashSet<int>(ids);
            var maxCount = 0;
            CharacterData bestMatch = null;
            foreach (CharacterData character in characters) {
                int count = GetHitboxes(character).Count(hitbox => idSet.Contains(hitbox.ID));
                if (count <= maxCount)
                    continue;
                maxCount = count;
                bestMatch = character;
            }
            return bestMatch;
        }

        /// <summary>
        /// Unity callback. Called when the EditorWindow is created.
        /// </summary>
        void OnEnable() {
            _characters = new ObjectSelector<CharacterData>(LoadAllCharacters());
            _characters.OnSelectedChange += UpdateHitboxSet;
            _filter = ~Hitbox.Type.Damageable;
        }

        void OnSelectionChange() {
            if (IsLocked)
                return;
            var newState = Selection.activeObject as AnimatorState;
            if (newState == State)
                return;
            State = Selection.activeObject as AnimatorState;
            SeekTime = 0f;
            Repaint();
        }

        void Refresh() {
            _characters.Selections = LoadAllCharacters();
            UpdateHitboxSet(_characters.Selected);
            Repaint();
        }

        void UpdateHitboxSet(CharacterData data) {
            _hitboxes = GetHitboxes(data);
            Array.Sort(_hitboxes, (h1, h2) => string.Compare(h1.name, h2.name, StringComparison.Ordinal));
        }

        void ToolbarGUI() {
            GUILayout.FlexibleSpace();
            _characters.Draw(GUIContent.none, EditorStyles.toolbarPopup, GUILayout.Width(100));
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton)) {
                Refresh();
            }
            _filter = (Hitbox.Type) EditorGUILayout.EnumMaskField(_filter, EditorStyles.toolbarPopup, GUILayout.Width(90));
        }

        void HitboxGUI(Hitbox hitbox) {
            if (hitbox == null)
                return;
            GUI.color = Config.Debug.GetHitboxColor(hitbox.CurrentType);
            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.LabelField(hitbox.name);
                GUI.color = Color.red;
                Rect rect = EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }

        void OnGUI() {
            using(new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
                ToolbarGUI();
            }
            using (new EditorGUILayout.VerticalScope()) {
                if(_hitboxes != null) {
                    foreach (Hitbox hitbox in _hitboxes) {
                       if((hitbox.CurrentType & _filter) != 0)
                           HitboxGUI(hitbox); 
                    }
                }
            }
            if (State == null)
                return;
            SeekTime = EditorGUILayout.Slider(GUIContent.none, SeekTime, 0f, 1f);
            foreach(StateMachineBehaviour behaviour in State.behaviours)
                EditorGUILayout.LabelField(behaviour.ToString());
        }

    }

}

