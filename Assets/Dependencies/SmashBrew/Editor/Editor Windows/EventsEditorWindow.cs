using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HouraiTeahouse.Editor;
using UnityEditor;
using UnityEditor.Animations;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.SmashBrew.Editor {

    internal class EventsEditorData {
        
        AnimatorState _state;
        List<Hitbox> _hitboxes;
        CharacterStateEvents _events;
        float _seekTime;

        public float SeekTime {
            get { return _seekTime; }
            set {
                if (_seekTime == value || State == null || Animator == null)
                    return;
                _seekTime = value;
                // TODO: Figure out a layer-agnostic way to do this
                Animator.Play(State.nameHash, 0, _seekTime);
                Animator.Update(0f);
            }
        }

        public Animator Animator {
            get {
                if (Spawn == null)
                    return null;
                return Spawn.GetComponentInChildren<Animator>();
            }
        }

        public event Action OnObjectUpdate;
        public event Action OnAssetEdit;

        public ObjectSelector<CharacterData> Characters { get; private set; }

        public CharacterStateEvents Events {
            get { return _events; }
            private set {
                bool changed = _events != value;
                _events = value;
                if (changed)
                    Characters.SetSelected(FindBestHitboxMatch(IDs,
                        Characters.Selections));
            }
        }

        public GameObject Spawn { get; private set; }

        public GameObject Target {
            get {
                GameObject target = Spawn;
                if (target == null && Characters.Selected && Characters.Selected.Prefab.Load())
                    return Characters.Selected.Prefab.Load();
                return null;
            }
        }

        List<int> IDs {
            get { return Events ? Events.IDs : null; }
        }

        List<HitboxKeyframe> Keyframes {
            get { return Events ? Events.Keyframes : null; }
        }

        public int HitboxCount {
            get { return IDs != null ? IDs.Count : 0; }
        }

        public int KeyframeCount {
            get { return Keyframes != null ? Keyframes.Count : 0; }
        }

        public IEnumerable<Hitbox> Hitboxes {
            get {
                return
                    _hitboxes.EmptyIfNull().Where(
                        h => h == null || (h.CurrentType & Filter) != 0);
            }
        }

        public Hitbox GetHitbox(int id) {
            if (_hitboxes == null)
                return null;
            return _hitboxes.Find(h => h.ID == id);
        }

        public void AddKeyframe(float time) {
            if(Keyframes == null)
                throw new InvalidOperationException();
            Check.Argument(Check.Range(time, 0f, 1f));
            Undo.RecordObject(Events, "Add Keyframe");
            var keyframe = new HitboxKeyframe {
                Time = time,
                States = new List<Hitbox.Type>()
            };
            for (var i = 0; i < HitboxCount; i++)
                keyframe.States.Add(Hitbox.Type.Inactive);
            Keyframes.Add(keyframe);
            OnEdit();
        }

        void OnEdit() {
            if (Events == null)
                return;
            Keyframes.Sort((k1, k2) => k1.Time.CompareTo(k2.Time));
            OnAssetEdit.SafeInvoke();
        }

        /// <summary>
        /// Adds a Hitbox to the set.
        /// </summary>
        /// <param name="hitbox">the Hitbox to add.</param>
        /// <returns>whether the element was added or not</returns>
        /// <exception cref="InvalidOperationException">instance does not point to a CharacterStateEvents instance</exception>
        /// <exception cref="ArgumentNullException"><paramref name="hitbox"/> is null</exception>
        public bool AddHitbox(Hitbox hitbox) {
            return AddID(Check.NotNull(hitbox).ID);
        }

        /// <summary>
        /// Adds an ID to the set.
        /// </summary>
        /// <param name="id">the ID to add.</param>
        /// <returns>whether the element was added or not</returns>
        /// <exception cref="InvalidOperationException">instance does not point to a CharacterStateEvents instance</exception>
        public bool AddID(int id) {
            if(IDs == null)
                throw new InvalidOperationException();
            if (IDs.Contains(id))
                return false;
            IDs.Add(id);
            foreach (HitboxKeyframe hitboxKeyframe in Keyframes) {
                hitboxKeyframe.States.Add(Hitbox.Type.Inactive);
            }
            OnEdit();
            return true;
        }

        public IEnumerable<KeyValuePair<float, Hitbox.Type>> GetProgression(int index) {
            if(Keyframes == null)
                throw new InvalidOperationException();
            Check.Argument(Check.Range(index, HitboxCount));
            foreach (HitboxKeyframe hitboxKeyframe in Keyframes) {
                yield return new KeyValuePair<float, Hitbox.Type>(hitboxKeyframe.Time, 
                    hitboxKeyframe.States[index]);
            }
        }

        public void SetState(int index, int keyframe, Hitbox.Type value) {
            if(IDs == null || Keyframes == null)
                throw new InvalidOperationException();
            Check.Argument(Check.Range(index, HitboxCount));
            Check.Argument(Check.Range(keyframe, KeyframeCount));
            Undo.RecordObject(Events, "Change Hitbox State");
            Keyframes[keyframe].States[index] = value;
            OnEdit();
        }

        public int GetID(int index) {
            if(IDs == null)
                throw new InvalidOperationException();
            Check.Argument(Check.Range(index, HitboxCount));
            return IDs[index];
        }

        public void DeleteKeyframe(int keyframe) {
            if(Keyframes == null)
                throw new InvalidOperationException();
            Check.Argument(Check.Range(keyframe, KeyframeCount));
            Undo.RecordObject(Events, "Delete Keyframe");
            Keyframes.RemoveAt(keyframe);
            OnEdit();
        }

        public void DeleteHitbox(int index) {
            if(IDs == null || Keyframes == null)
                throw new InvalidOperationException();
            Check.NotNull(Check.Range(index, HitboxCount));
            Undo.RecordObject(Events, "Delete Hitbox");
            IDs.RemoveAt(index);
            foreach (HitboxKeyframe hitboxKeyframe in Keyframes)
                hitboxKeyframe.States.RemoveAt(index);
            OnEdit();
        }

        public AnimatorState State {
            get { return _state; }
            set {
                _state = value;
                Events = _state
                    ? _state.GetBehaviour<CharacterStateEvents>()
                    : null;
                OnEdit();
                OnObjectUpdate.SafeInvoke();
            }
        }

        public Hitbox.Type Filter { get; set; }

        public EventsEditorData() {
            Characters = new ObjectSelector<CharacterData>();
            Characters.OnSelectedChange += UpdateCharacter;
            Refresh();
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

        public void Refresh() {
            var selected = Characters.Selected;
            Characters.Selections = LoadAllCharacters();
            UpdateCharacter(selected, Characters.Selected);
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

        public void UpdateCharacter(CharacterData oldCharacter, CharacterData data) {
            if(oldCharacter != null)
                oldCharacter.Prefab.Unload();
            if (Spawn != null) {
                GameObject newInstance = SpawnPrefab();
                newInstance.transform.Copy(Spawn.transform);
                Undo.DestroyObjectImmediate(Spawn);
                Spawn = newInstance;
            }
            _hitboxes = new List<Hitbox>(GetHitboxes(data).EmptyIfNull());
            _hitboxes.Sort((h1, h2) => string.Compare(h1.name, h2.name, StringComparison.Ordinal));
        }

        public void ToggleSpawn(bool toggle) {
            if(!toggle && Spawn)
                Undo.DestroyObjectImmediate(Spawn);
            if (toggle && !Spawn)
                Spawn = SpawnPrefab();
        }

        GameObject SpawnPrefab() {
            GameObject prefab = Characters.Selected.Prefab.Load();

            var instance = (GameObject) PrefabUtility.InstantiatePrefab(prefab);
            Undo.RegisterCreatedObjectUndo(instance, string.Format("Spawn {0}", prefab.name));
            return instance;
        }
    }

    public class EventsEditorWindow : LockableEditorWindow {

        EventsEditorData Data;

        ObjectSelector<CharacterData> Characters {
            get {
                if(Data == null)
                    Data = new EventsEditorData();
                return Data.Characters;
            }
        }

        Hitbox.Type state = Hitbox.Type.Offensive;

        static readonly string filterPref = "eventEditorFilter";

        public static EventsEditorWindow GetWindow() {
            return GetWindow<EventsEditorWindow>("Events");
        }

        [MenuItem("Window/Animator Events Editor")]
        static void CreateWindow() {
            EventsEditorWindow window = GetWindow();
            window.Show();
            window.Repaint();
        }

        /// <summary>
        /// Unity callback. Called when the EditorWindow is created.
        /// </summary>
        void OnEnable() {
            Data = new EventsEditorData();
            if (EditorPrefs.HasKey(filterPref))
                Data.Filter = (Hitbox.Type) EditorPrefs.GetInt(filterPref);
            else {
                Data.Filter = (Hitbox.Type) ~0;
                EditorPrefs.SetInt(filterPref, (int) Data.Filter);
            }
            Data.OnAssetEdit += Repaint;
        }

        void OnDestroy() {
            EditorPrefs.SetInt(filterPref, (int) Data.Filter);
        }

        void OnSelectionChange() {
            if (IsLocked || Data == null)
                return;
            var newState = Selection.objects.OfType<AnimatorState>().FirstOrDefault();
            if (newState == null || newState == Data.State)
                return;
            Data.State = newState;
            Data.SeekTime = 0f;
            Repaint();
        }

        void OnHierarchyChange() {
            Repaint();
        }

        void SpawnButton() {
            GUI.enabled = Characters.Selected != null &&
                    Characters.Selected.Prefab.Load() != null;
            Data.ToggleSpawn(GUILayout.Toggle(Data.Spawn, "Spawn", EditorStyles.toolbarButton, GUILayout.Width(40)));
            GUI.enabled = true;
        }

        void ToolbarGUI() {
            SpawnButton();
            GUILayout.FlexibleSpace();
            Characters.Draw(GUIContent.none, EditorStyles.toolbarPopup, GUILayout.Width(100));
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton)) {
                Data.Refresh();
                Repaint();
            }

            var oldColor = GUI.color;
            GUI.color = Config.Debug.GetHitboxColor(state);
            state =
                (Hitbox.Type)
                    EditorGUILayout.EnumPopup(
                        state,
                        EditorStyles.toolbarPopup,
                        GUILayout.Width(100));
            GUI.color = oldColor;
            Data.Filter = (Hitbox.Type) EditorGUILayout.EnumMaskField(Data.Filter, EditorStyles.toolbarPopup, GUILayout.Width(100));
        }

        IEnumerable<Hitbox> GetObjectHitboxes(IEnumerable<Object> objects) {
            return objects.OfType<GameObject>().GetComponents<Hitbox>();
        }

        void HandleEvents(Rect rect) {
            Event currentEvent = Event.current;
            switch (currentEvent.type) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                case EventType.DragExited:
                    if (!rect.Contains(currentEvent.mousePosition))
                        return;
                    break;
            }
            switch (currentEvent.type) {
                case EventType.DragUpdated:
                    DragAndDrop.visualMode = Data.Events && GetObjectHitboxes(DragAndDrop.objectReferences).Any() ? 
                        DragAndDropVisualMode.Generic : DragAndDropVisualMode.None;
                    break;
                case EventType.DragExited:
                case EventType.DragPerform:
                    DragAndDrop.AcceptDrag();
                    foreach (Hitbox hb in GetObjectHitboxes(DragAndDrop.objectReferences)) 
                        Data.AddHitbox(hb);
                    break;
            }
        }

        void DrawHitboxGUI (int index) {
            using (new EditorGUILayout.HorizontalScope()) {
                int id = Data.GetID(index);
                var hitbox = Data.GetHitbox(id);
                string display = "<color=red>{0}</color>".With(id);
                if (hitbox != null)
                    display = hitbox.name;
                var style = GUI.skin.label;
                style.richText = true;
                style.alignment = TextAnchor.MiddleLeft;
                EditorGUILayout.LabelField(display, style, GUILayout.MaxWidth(150));
                Color oldColor = GUI.color;
                GUI.color = Color.black;
                Rect area = EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                float width = area.width;
                var lastKeyframe = 0f;
                if(Event.current.type != EventType.Layout) {
                    float delta;
                    Rect stateRect;
                    int i = 0;
                    foreach (var data in Data.GetProgression(index)) {
                        float time = data.Key;
                        delta = time - lastKeyframe;
                        stateRect = new Rect(area);
                        stateRect.x += lastKeyframe * width;
                        stateRect.width = delta * width;
                        GUI.color = Config.Debug.GetHitboxColor(data.Value);
                        GUI.Box(stateRect, GUIContent.none, EditorStyles.toolbar);
                        EditorGUI.DrawRect(new Rect(stateRect.x - 1, stateRect.y, 2, stateRect.height), Color.black);
                        Event evt = Event.current;
                        if (stateRect.Contains(evt.mousePosition)) {
                            switch (evt.type) {
                                case EventType.MouseDown:
                                case EventType.MouseDrag:
                                    Data.SetState(index, i, state);
                                    break;
                            }
                        }
                        lastKeyframe = time;
                        i++;
                    }
                    delta = 1 - lastKeyframe;
                    stateRect = new Rect(area);
                    stateRect.x += lastKeyframe * width;
                    stateRect.width = delta * width;
                    GUI.color = Color.red;
                    EditorGUI.DrawRect(new Rect(stateRect.x - 1, stateRect.y, 2, stateRect.height), Color.black);
                    GUI.Box(stateRect, GUIContent.none, EditorStyles.toolbar);
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                GUI.color = oldColor;
            }
        }

        void OnGUI() {
            using(new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
                ToolbarGUI();
            }
            Rect area;
            using(new EditorGUILayout.HorizontalScope()) {
                Data.SeekTime = EditorGUILayout.Slider(Data.SeekTime, 0, 1);
                if(GUILayout.Button("Add"))
                    Data.AddKeyframe(Data.SeekTime);
            }
            using (var scope = new EditorGUILayout.VerticalScope()) {
                area = scope.rect;
                string error = null;
                if(Data.Events == null) {
                    error = "No selected event. Select an event.";
                } else if (Data.Events.IDs.Count <= 0) {
                    error =
                        "No Hitboxes on {0}. Drag Character hitboxes here.".With
                            (Data.State.name);
                }
                if (!error.IsNullOrEmpty()) {
                    var style = GUI.skin.label;
                    style.alignment = TextAnchor.MiddleCenter;
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField(error, style);
                    GUILayout.FlexibleSpace();
                } else {
                    for(var i = 0; i < Data.HitboxCount; i++) 
                        DrawHitboxGUI(i);
                    GUILayout.FlexibleSpace();
                }
            }
            HandleEvents(area);
        }

    }

}

