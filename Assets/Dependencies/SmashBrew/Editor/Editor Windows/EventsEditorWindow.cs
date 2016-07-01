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

        Hitbox[] _hitboxes;

        public event Action OnObjectUpdate;
        public event Action OnAssetEdit;

        public ObjectSelector<CharacterData> Characters { get; private set; }
        public CharacterStateEvents Events { get; private set; }
        public GameObject Spawn { get; private set; }

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

        public void AddKeyframe(float time) {
            if(Keyframes == null)
                throw new InvalidOperationException();
            Check.Argument(Check.Range(time, 0f, 1f));
            var keyframe = new HitboxKeyframe();
            keyframe.Time = time;
            keyframe.States = new List<bool>();
            for (var i = 0; i < HitboxCount; i++)
                keyframe.States.Add(false);
            Keyframes.Add(keyframe);
            OnEdit();
        }

        void OnEdit() {
            if (Events == null)
                return;
            Keyframes.Sort((k1, k2) => k1.Time.CompareTo(k2.Time));
            EditorUtility.SetDirty(Events);
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
                hitboxKeyframe.States.Add(false);
            }
            OnEdit();
            return true;
        }

        public IEnumerable<KeyValuePair<float, bool>> GetProgression(int index) {
            if(Keyframes == null)
                throw new InvalidOperationException();
            Check.Argument(Check.Range(index, HitboxCount));
            foreach (HitboxKeyframe hitboxKeyframe in Keyframes) {
                yield return new KeyValuePair<float, bool>(hitboxKeyframe.Time, 
                    hitboxKeyframe.States[index]);
            }
        }

        public void SetState(int index, int keyframe, bool value) {
            if(IDs == null || Keyframes == null)
                throw new InvalidOperationException();
            Check.Argument(Check.Range(index, HitboxCount));
            Check.Argument(Check.Range(keyframe, KeyframeCount));
            Keyframes[keyframe].States[index] = value;
            OnEdit();
        }

        public int GetID(int index) {
            if(IDs == null)
                throw new InvalidOperationException();
            Check.Argument(Check.Range(index, HitboxCount));
            return IDs[index];
        }

        public bool ToggleState(int index, int keyframe) {
            if (IDs == null || Keyframes == null)
                throw new InvalidOperationException();
            Check.Argument(Check.Range(index, HitboxCount));
            Check.Argument(Check.Range(keyframe, KeyframeCount));
            var states = Keyframes[keyframe].States;
            states[index] = !states[index];
            OnEdit();
            return states[index];
        }

        public void DeleteKeyframe(int keyframe) {
            if(Keyframes == null)
                throw new InvalidOperationException();
            Check.Argument(Check.Range(keyframe, KeyframeCount));
            Keyframes.RemoveAt(keyframe);
            OnEdit();
        }

        public void DeleteHitbox(int index) {
            if(IDs == null || Keyframes == null)
                throw new InvalidOperationException();
            Check.NotNull(Check.Range(index, HitboxCount));
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
            Characters.Selections = LoadAllCharacters();
            UpdateCharacter(Characters.Selected);
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

        public void UpdateCharacter(CharacterData data) {
            if (Spawn != null) {
                GameObject newInstance = SpawnPrefab();
                newInstance.transform.Copy(Spawn.transform);
                Undo.DestroyObjectImmediate(Spawn);
                Spawn = newInstance;
            }
            _hitboxes = GetHitboxes(data);
            if(_hitboxes != null)
                Array.Sort(_hitboxes, (h1, h2) => string.Compare(h1.name, h2.name, StringComparison.Ordinal));
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

        float SeekTime { get; set; }
        bool state;

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
            var newState = Selection.activeObject as AnimatorState;
            if (newState == null || newState == Data.State)
                return;
            Data.State = newState;
            SeekTime = 0f;
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
            Data.Filter = (Hitbox.Type) EditorGUILayout.EnumMaskField(Data.Filter, EditorStyles.toolbarPopup, GUILayout.Width(90));
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
                EditorGUILayout.LabelField(Data.GetID(index).ToString(), GUILayout.Width(100));
                Color oldColor = GUI.color;
                GUI.color = Color.black;
                Rect area = EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                float width = area.width;
                float start = area.x;
                var lastKeyframe = 0f;
                if(Event.current.type != EventType.Layout) {
                    float delta, time;
                    Rect stateRect;
                    int i = 0;
                    foreach (var data in Data.GetProgression(index)) {
                        time = data.Key;
                        delta = time - lastKeyframe;
                        stateRect = new Rect(area);
                        stateRect.x += lastKeyframe * width;
                        stateRect.width = delta * width;
                        GUI.color = data.Value ? Color.green : Color.red;
                        GUI.Box(stateRect, GUIContent.none, EditorStyles.toolbar);
                        EditorGUI.DrawRect(new Rect(stateRect.x - 1, stateRect.y, 2, stateRect.height), Color.black);
                        Event evt = Event.current;
                        if (stateRect.Contains(evt.mousePosition)) {
                            switch (evt.type) {
                                case EventType.MouseDown:
                                    state = Data.ToggleState(index, i);
                                    break;
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

        float insert;

        void OnGUI() {
            using(new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
                ToolbarGUI();
            }
            Rect area;
            using(new EditorGUILayout.HorizontalScope()) {
                insert = EditorGUILayout.Slider(insert, 0, 1);
                if(GUILayout.Button("Add"))
                    Data.AddKeyframe(insert);
            }
            using (var scope = new EditorGUILayout.VerticalScope()) {
                area = scope.rect;
                for(var i = 0; i < Data.HitboxCount; i++) 
                    DrawHitboxGUI(i);
                GUILayout.FlexibleSpace();
            }
            HandleEvents(area);
        }

    }

}

