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
        CharacterStateEvents _events;
        List<Hitbox> _hitboxes;
        float _seekTime;

        public float SeekTime {
            get { return _seekTime; }
            set {
                if(_seekTime == value)
                    return;
                _seekTime = Mathf.Clamp01(value);
                if (State == null || Animator == null)
                    return;
                // TODO: Figure out a layer-agnostic way to do this
                Animator.Play(State.nameHash, 0, _seekTime);
                Animator.Update(0f);
                UpdateSpawn();
            }
        }

        public void UpdateSpawn() {
            if (Spawn == null)
                return;
            HitboxKeyframe frame = Keyframes.FindLast(h => h.Time < SeekTime);
            for (var i = 0; i < IDs.Count; i++) {
                var hitbox = GetHitbox(IDs[i]);
                if (hitbox == null)
                    continue;
                if (frame != null)
                    hitbox.CurrentType = frame.States[i];
                else
                    hitbox.CurrentType = hitbox.DefaultType;
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
                if (target != null)
                    return target;
                if (Characters.Selected && Characters.Selected.Prefab.Load())
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

        public List<Hitbox> Hitboxes {
            get {
                if(Target == null)
                    return new List<Hitbox>();
                return new List<Hitbox>(Target.GetComponentsInChildren<Hitbox>());
            }
        }

        public Hitbox GetHitbox(int id) {
            if (Hitboxes == null)
                return null;
            return Hitboxes.Find(h => h.ID == id);
        }

        public Hitbox.Type GetType(int id) {
            Hitbox hitbox = GetHitbox(id);
            return hitbox != null ? hitbox.DefaultType : Hitbox.Type.Inactive;
        }

        public void AddKeyframe(float time) {
            if(Keyframes == null)
                throw new InvalidOperationException();
            Check.Argument(Check.Range(time, 0f, 1f));
            Undo.RecordObject(Events, "Add Keyframe");
            HitboxKeyframe prevKeyframe = PrevKeyframe(time);
            var keyframe = new HitboxKeyframe {
                Time = time,
            };
            if(prevKeyframe != null) {
                keyframe.States = new List<Hitbox.Type>(prevKeyframe.States);
            } else {
                keyframe.States = IDs.Select<int, Hitbox.Type>(GetType).ToList();
            }
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
            Hitbox.Type type = GetType(id);
            foreach (HitboxKeyframe hitboxKeyframe in Keyframes)
                hitboxKeyframe.States.Add(type);
            OnEdit();
            return true;
        }

        public IEnumerable<KeyValuePair<float, Hitbox.Type>> GetProgression(int index) {
            if(Keyframes == null)
                throw new InvalidOperationException();
            Check.Argument(Check.Range(index, HitboxCount));
            Hitbox hitbox = GetHitbox(IDs[index]);
            Hitbox.Type type = hitbox != null
                ? hitbox.DefaultType
                : Hitbox.Type.Inactive;
            foreach (HitboxKeyframe hitboxKeyframe in Keyframes) {
                yield return new KeyValuePair<float, Hitbox.Type>(hitboxKeyframe.Time, type);
                type = hitboxKeyframe.States[index];
            }
            yield return new KeyValuePair<float, Hitbox.Type>(1.0f, type);
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

        public HitboxKeyframe NextKeyframe(float time) {
            return Keyframes.Find(kf => kf.Time > time);
        }

        public HitboxKeyframe PrevKeyframe(float time) {
            return Keyframes.FindLast(kf => kf.Time < time);
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

        public EventsEditorData() {
            Characters = new ObjectSelector<CharacterData>();
            Characters.OnSelectedChange += UpdateCharacter;
            Refresh();
        }

        // Loads all of the Characters from Resources.
        static CharacterData[] LoadAllCharacters() {
            return Resources.LoadAll<CharacterData>(string.Empty);
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
                int count = character.Prefab.Load()
                                .GetComponentsInChildren<Hitbox>()
                                .Count(hitbox => idSet.Contains(hitbox.ID));
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
            _hitboxes = new List<Hitbox>(Target == null ? Enumerable.Empty<Hitbox>() : Target.GetComponentsInChildren<Hitbox>());
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
        Rect seekArea;
        int index, i;
        EventListener DragListener;
        EventListener SeekListener;
        EventListener SetListener;
        bool play;
        const float elementHeight = 18;

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

            DragListener = new EventListener();
            DragListener.DragUpdated += delegate {
                DragAndDrop.visualMode = Data.Events && GetObjectHitboxes(DragAndDrop.objectReferences).Any() ? 
                    DragAndDropVisualMode.Generic : DragAndDropVisualMode.None;
            }; 
            DragListener.AddListeners(new [] {EventType.DragExited, EventType.DragPerform},
                delegate {
                    DragAndDrop.AcceptDrag();
                    foreach (Hitbox hb in GetObjectHitboxes(DragAndDrop.objectReferences)) 
                        Data.AddHitbox(hb);
                });

            SeekListener = new EventListener();
            SeekListener.AddListeners(new [] {EventType.MouseDown, EventType.MouseDrag },
                delegate (Event evt) {
                    Vector2 pos = evt.mousePosition;
                    Data.SeekTime = (pos.x - seekArea.x)
                        / seekArea.width;
                    Repaint();
                });

            SetListener = new EventListener();
            SetListener.AddListeners(new [] {EventType.MouseDown, EventType.MouseDrag},
                delegate {
                    changes.Add(new KeyValuePair<int, int>(index, i));
                });

            changes = new List<KeyValuePair<int, int>>();
            Data.OnAssetEdit += Repaint;
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

        void Update() {
            if (!play || Data.State == null || !(Data.State.motion is AnimationClip))
                return;
            var clip = (AnimationClip) Data.State.motion;
            float time = Data.SeekTime + 1 / (200 * clip.length);
            if (time > 1f)
                time = 0f;
            Data.SeekTime = time;
            Repaint();
        }

        List<KeyValuePair<int, int>> changes;

        void ToolbarGUI() {
            using (
                hGUI.Enabled(Characters.Selected != null
                    && Characters.Selected.Prefab.Load())) {
                if (hGUI.ToolbarButton(hGUI.BuiltinContent("Animation.PrevKey"))) {
                    HitboxKeyframe nextKeyframe = Data.NextKeyframe(Data.SeekTime);
                    Data.SeekTime = nextKeyframe != null ? nextKeyframe.Time : 1.0f;
                }
                play = hGUI.ToolbarToggle(hGUI.BuiltinContent("Animation.Play"), play);
                if (hGUI.ToolbarButton(hGUI.BuiltinContent("Animation.NextKey"))) {
                    HitboxKeyframe prevKeyframe = Data.PrevKeyframe(Data.SeekTime);
                    Data.SeekTime = prevKeyframe != null ? prevKeyframe.Time : 0.0f;
                }
                if(hGUI.ToolbarButton(hGUI.BuiltinContent("Animation.AddKeyframe")))
                    Data.AddKeyframe(Data.SeekTime);
                if(hGUI.ToolbarButton(hGUI.BuiltinContent("Animation.AddEvent"))) {
                }
                Data.ToggleSpawn(hGUI.ToolbarToggle("Spawn", Data.Spawn, GUILayout.Width(40)));
            }
            hGUI.Space();
            if (GUILayout.Button("Refresh", Style.ToolbarButton)) {
                Data.Refresh();
                Repaint();
            }
            Characters.Draw(GUIContent.none, Style.ToolbarPopup, GUILayout.Width(100));
        }

        IEnumerable<Hitbox> GetObjectHitboxes(IEnumerable<Object> objects) {
            return objects.OfType<GameObject>().GetComponents<Hitbox>();
        }

        void DrawHitboxLabel(int index) {
            using (hGUI.Horizontal(GUILayout.Height(elementHeight))) {
                int id = Data.GetID(index);
                var hitbox = Data.GetHitbox(id);
                string display = "<color=red>{0}</color>".With(id);
                if (hitbox != null)
                    display = hitbox.name;
                EditorGUILayout.LabelField(display, GUI.skin.label.WithRichText().WithAlignment(TextAnchor.MiddleLeft), GUILayout.MaxWidth(150));
                if(GUILayout.Button(GUIContent.none, Style.Minus))
                    Data.DeleteHitbox(index);
            }
        }

        void DrawHitboxGUI (int index) {
            using (var scope = hGUI.Horizontal(GUILayout.Height(elementHeight))) {
                float width = scope.rect.width;
                var lastKeyframe = 0f;
                i = -1;
                this.index = index;
                foreach (var data in Data.GetProgression(index)) {
                    float time = data.Key;
                    float delta = time - lastKeyframe;
                    var stateRect = new Rect(scope.rect);
                    stateRect.x += lastKeyframe * width;
                    stateRect.width = delta * width;
                    GUI.color = Config.Debug.GetHitboxColor(data.Value);
                    GUI.Box(stateRect, GUIContent.none, Style.Toolbar);
                    EditorGUI.DrawRect(new Rect(stateRect.x - 1, stateRect.y, 1, stateRect.height), Color.black);
                    if(Check.Range(i, Data.KeyframeCount))
                        SetListener.EventCheck(stateRect);
                    lastKeyframe = time;
                    i++;
                }
                GUILayout.FlexibleSpace();
            }
        }

        void OnGUI() {
            using(hGUI.Horizontal(Style.Toolbar)) {
                ToolbarGUI();
            }
            using (hGUI.Horizontal()) {
                using (hGUI.Vertical(EditorStyles.helpBox.WithoutMargins().WithoutPadding(),
                                        GUILayout.Width(150), 
                                        GUILayout.ExpandHeight(true))) {
                    using(hGUI.Color(Config.Debug.GetHitboxColor(state))) {
                        state =
                            (Hitbox.Type)
                                EditorGUILayout.EnumPopup(
                                    state,
                                    Style.ToolbarPopup,
                                    GUILayout.Height(18));
                    }
                    for(var i = 0; i < Data.HitboxCount; i++) 
                        DrawHitboxLabel(i);
                    hGUI.Space();
                }
                using (var scope = hGUI.Vertical()) {
                    seekArea =
                        EditorGUILayout.BeginHorizontal(
                            Style.AnimationEventBackground,
                            GUILayout.Height(18));
                    SeekListener.EventCheck(seekArea);
                    hGUI.Space();
                    EditorGUILayout.EndHorizontal();
                    using (var displayScope = hGUI.Vertical()) {
                        DragListener.EventCheck(displayScope.rect);
                        string error = null;
                        if(Data.Events == null) {
                            error = "No selected event. Select an event.";
                        } else if (Data.Events.IDs.Count <= 0) {
                            error =
                                "No Hitboxes on {0}. Drag Character hitboxes here.".With
                                    (Data.State.name);
                        }
                        if (!error.IsNullOrEmpty()) {
                            hGUI.Space();
                            EditorGUILayout.LabelField(error, GUI.skin.label.WithAlignment(TextAnchor.MiddleCenter));
                        } else {
                            for(var i = 0; i < Data.HitboxCount; i++) 
                                DrawHitboxGUI(i);
                        }
                        hGUI.Space();
                    }
                    var displayRect = new Rect(scope.rect);
                    displayRect.x += Data.SeekTime * displayRect.width;
                    displayRect.width = 1;
                    EditorGUI.DrawRect(displayRect, Color.red);
                }
            }
            if(changes.Count > 0) {
                foreach (KeyValuePair<int, int> change in changes) {
                    Data.SetState(change.Key, change.Value, state);
                }
                Data.UpdateSpawn();
                changes.Clear();
            }
        }
    }
}

