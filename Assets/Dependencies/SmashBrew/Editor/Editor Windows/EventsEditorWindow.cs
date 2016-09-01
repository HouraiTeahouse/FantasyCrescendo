using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HouraiTeahouse.Editor;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.SmashBrew.Editor {

    public class EventsEditorWindow : LockableEditorWindow {

        struct Edit {
            public int index;
            public int keyframe;
            public Hitbox.Type type;
        }

        enum DragStart {
            None,
            SeekBar,
            EditArea,
            Keyframe,
            Hitbox
        }

        static readonly Color KeyframeLineColor = Color.black;
        static readonly Color SeekLineColor = Color.red;
        static readonly Color SelectedColor = Color.blue;

        Hitbox.Type state = Hitbox.Type.Offensive;
        DragStart dragStart = DragStart.None;
        HitboxKeyframe selectedKeyframe;
        AnimatorState _state;
        CharacterStateEvents _behaviour;
        List<Hitbox> _hitboxes;
        List<Edit> changes;
        float _seekTime;
        Rect seekArea;
        int hitboxIndex, keyframeIndex;
        EventListener DragListener;
        EventListener SeekListener;
        EventListener SetListener;
        EventListener GeneralEventListener;
        EventListener KeyframeListener;
        EventListener HitboxListener;
        bool play;
        const float elementHeight = 17;

        public static EventsEditorWindow GetWindow() {
            return GetWindow<EventsEditorWindow>("Events");
        }

        [MenuItem("Window/Animator Behaviour Editor")]
        static void CreateWindow() {
            EventsEditorWindow window = GetWindow();
            window.Show();
            window.Repaint();
        }

        public float SeekTime {
            get { return _seekTime; }
            set {
                if(_seekTime == value)
                    return;
                _seekTime = Mathf.Clamp01(value);
                if (State == null || Animator == null)
                    return;
                // TODO: Figure out a layer-agnostic way to do thisGUILayout.ExpandHeight(true)
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
                hitbox.CurrentType = frame != null ? 
                                        frame.States[i] : 
                                        hitbox.DefaultType;
            }
            SceneView.RepaintAll();
        }

        public Animator Animator {
            get {
                if (Spawn == null)
                    return null;
                return Spawn.GetComponentInChildren<Animator>();
            }
        }

        event Action OnAssetEdit;

        ObjectSelector<CharacterData> Characters { get; set; }

        CharacterStateEvents Behaviour {
            get { return _behaviour; }
            set {
                bool changed = _behaviour != value;
                _behaviour = value;
                if (_behaviour != null && _behaviour.Data != null)
                    Events = _behaviour.Data;
                if (changed && Events != null)
                    Characters.SetSelected(Events.FindBestHitboxMatch(
                        Characters.Selections));
            }
        }

        EventData Events { get; set; }

        GameObject Spawn { get; set; }

        GameObject Target {
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

        List<AnimationEvent> OtherEvents {
            get { return Events ? Events.OtherEvents : null; }
        }

        public List<Hitbox> Hitboxes {
            get {
                if(Target == null)
                    return new List<Hitbox>();
                return new List<Hitbox>(Target.GetComponentsInChildren<Hitbox>());
            }
        }

        Hitbox GetHitbox(int id) {
            if (Hitboxes == null)
                return null;
            return Hitboxes.Find(h => h.ID == id);
        }

        Hitbox.Type GetType(int id) {
            Hitbox hitbox = GetHitbox(id);
            return hitbox != null ? hitbox.DefaultType : Hitbox.Type.Inactive;
        }

        void OnEdit() {
            if(Keyframes != null)
                Keyframes.Sort((k1, k2) => k1.Time.CompareTo(k2.Time));
            OnAssetEdit.SafeInvoke();
        }

        Hitbox.Type GetTypeOrDefault(int index) {
            Hitbox hitbox = GetHitbox(IDs[index]);
            return hitbox != null
                ? hitbox.DefaultType
                : Hitbox.Type.Inactive;
        }

        public IEnumerable<KeyValuePair<float, Hitbox.Type>> GetProgression(int index) {
            if(Keyframes == null)
                throw new InvalidOperationException();
            Check.Argument(Check.Range(index, IDs));
            var type = GetTypeOrDefault(index);
            foreach (HitboxKeyframe hitboxKeyframe in Keyframes) {
                yield return new KeyValuePair<float, Hitbox.Type>(hitboxKeyframe.Time, type);
                type = hitboxKeyframe.States[index];
            }
            yield return new KeyValuePair<float, Hitbox.Type>(1.0f, type);
        }

        public AnimatorState State {
            get { return _state; }
            set {
                _state = value;
                Behaviour = _state
                    ? _state.GetBehaviour<CharacterStateEvents>()
                    : null;
                OnEdit();
            }
        }

        // Loads all of the Characters from Resources.
        static CharacterData[] LoadAllCharacters() {
            return Resources.LoadAll<CharacterData>(string.Empty);
        }

        void Refresh() {
            var selected = Characters.Selected;
            Characters.Selections = LoadAllCharacters();
            UpdateCharacter(selected, Characters.Selected);
            Repaint();
        }

        void UpdateCharacter(CharacterData oldCharacter, CharacterData data) {
            if (oldCharacter == data)
                return;
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

        void ToggleSpawn(bool toggle) {
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

        void SetSeekTime(Event evt) {
            Vector2 pos = evt.mousePosition;
            SeekTime = Mathf.Clamp01((pos.x - seekArea.x) / seekArea.width);
            Repaint();
        }

        void SetKeyframeTime(Event evt, HitboxKeyframe keyframe) {
            Assert.IsNotNull(Events);
            Assert.IsNotNull(keyframe);
            Vector2 pos = evt.mousePosition;
            keyframe.Time = Mathf.Clamp01((pos.x - seekArea.x) / seekArea.width);
            Events.Sortkeyframes();
            Repaint();
        }

        #region Unity Callbacks
        /// <summary>
        /// Unity callback. Called when the EditorWindow is created.
        /// </summary>
        void OnEnable() {
            changes = new List<Edit>();
            DragListener = new EventListener()
                .AddListener(EventType.DragUpdated,delegate {
                        DragAndDrop.visualMode = Behaviour && GetObjectHitboxes(DragAndDrop.objectReferences).Any() ? 
                            DragAndDropVisualMode.Generic : DragAndDropVisualMode.None;
                    })
                .AddListeners(new [] {EventType.DragExited, EventType.DragPerform},
                    delegate {
                        if (Events == null)
                            return;
                        DragAndDrop.AcceptDrag();
                        foreach (Hitbox hb in GetObjectHitboxes(DragAndDrop.objectReferences)) 
                            Events.AddHitbox(hb);
                    });

            GeneralEventListener = new EventListener()
                .AddListener(EventType.MouseDrag, 
                    delegate(Event evt) {
                        if(dragStart == DragStart.SeekBar)
                            SetSeekTime(evt);
                        if(dragStart == DragStart.Keyframe)
                            SetKeyframeTime(evt, selectedKeyframe);
                    })
                .AddListener(EventType.MouseUp, delegate { dragStart = DragStart.None; })
                .AddListener(EventType.KeyDown,
                    delegate (Event evt) {
                        if (evt.keyCode == KeyCode.Delete
                            && selectedKeyframe != null) {
                            Events.DeleteKeyframe(selectedKeyframe);
                            Repaint();
                        }
                    });

            SeekListener = new EventListener()
                .AddListeners(new [] {EventType.MouseDown, EventType.MouseDrag },
                    delegate (Event evt) {
                        if(evt.type == EventType.MouseDown)
                            dragStart = DragStart.SeekBar;
                        if (evt.type == EventType.MouseDrag
                            && dragStart != DragStart.SeekBar)
                            return;
                        SetSeekTime(evt);
                    });

            SetListener = new EventListener()
                .AddListeners(new[] {EventType.MouseDown, EventType.MouseDrag},
                    delegate(Event evt) {
                        if(evt.type == EventType.MouseDown)
                            dragStart = DragStart.EditArea;
                        if (evt.type == EventType.MouseDrag
                            && dragStart != DragStart.EditArea)
                            return;
                        var type = evt.button == 0 ? state : GetTypeOrDefault(hitboxIndex);
                        if (Events == null || Events.GetState(hitboxIndex, keyframeIndex) == type)
                            return;
                        changes.Add(new Edit {
                            index = hitboxIndex,
                            keyframe = keyframeIndex,
                            type = type
                        });
                    });

            KeyframeListener = new EventListener()
                .AddListener(EventType.MouseDown,
                    delegate {
                        dragStart = DragStart.Keyframe;
                        selectedKeyframe = Keyframes[keyframeIndex];
                    });

            HitboxListener = new EventListener()
                .AddListener(EventType.MouseDown,
                    delegate {
                        var hitbox = GetHitbox(IDs[hitboxIndex]);
                        if (hitbox != null)
                            Selection.activeGameObject = hitbox.gameObject;
                    });

            OnAssetEdit += Repaint;

            Characters = new ObjectSelector<CharacterData>(c => c.name, c => c != null);

            Characters.SelectionChanged += UpdateCharacter;
            Refresh();
        }

        void OnSelectionChange() {
            if (!IsLocked) {
                var newState = Selection.objects.OfType<AnimatorState>().FirstOrDefault();
                if (newState != null && newState != State)
                    State = newState;
            }
            Repaint();
        }

        void OnHierarchyChange() {
            Repaint();
        }

        void Update() {
            if (!play || State == null || !(State.motion is AnimationClip))
                return;
            var clip = (AnimationClip) State.motion;
            float time = SeekTime + 1 / (200 * clip.length) * State.speed;
            if (time > 1f)
                time = 0f;
            SeekTime = time;
            Repaint();
        }

        public override void AddItemsToMenu(GenericMenu menu) {
            base.AddItemsToMenu(menu);
            menu.AddItem(new GUIContent("Refresh"), false, Refresh);
        }
        #endregion

        IEnumerable<Hitbox> GetObjectHitboxes(IEnumerable<Object> objects) {
            return objects.OfType<GameObject>().GetComponents<Hitbox>();
        }

        #region GUI Drawing Code
        void OnGUI() {
            using(hGUI.Horizontal(EditorStyles.toolbar))
                ToolbarGUI();
            using (hGUI.Horizontal()) {
                LabelGUI();
                TimeGUI();
            }
            GeneralEventListener.EventCheck();
            if(changes.Count > 0) {
                foreach (Edit change in changes)
                    Events.SetState(change.index, change.keyframe, change.type);
                UpdateSpawn();
                changes.Clear();
                Repaint();
            }
        }

        void TimeGUI() {
            using (var scope = hGUI.Vertical()) {
                SeekAreaGUI();
                using (var displayScope = hGUI.Vertical()) {
                    DragListener.EventCheck(displayScope.rect);
                    string error = null;
                    if(IDs == null) {
                        error = "No selected event. Select an event.";
                    } else if (IDs.Count <= 0) {
                        error =
                            "No Hitboxes on {0}. Drag Character hitboxes here.".With
                                (State.name);
                    }
                    if (!error.IsNullOrEmpty()) {
                        hGUI.Space();
                        EditorGUILayout.LabelField(error, GUI.skin.label.WithAlignment(TextAnchor.MiddleCenter));
                    } else {
                        for(var i = 0; i < IDs.Count; i++) 
                            DrawHitboxGUI(i);
                    }
                    hGUI.Space();
                }
                Rect areaRect = scope.rect;
                DrawVerticalLine(SeekTime, SeekLineColor, areaRect);
                areaRect.height -= elementHeight;
                areaRect.y += elementHeight;
                foreach (HitboxKeyframe keyframe in Keyframes.IgnoreNulls())
                    DrawVerticalLine(keyframe.Time, KeyframeLineColor, areaRect);
            }
        }

        bool ToolbarButton(string builtIn) {
            return hGUI.ToolbarButton(hGUI.BuiltinContent(builtIn));
        }

        bool ToolbarToggle(string builtIn, bool value) {
            return hGUI.ToolbarToggle(hGUI.BuiltinContent(builtIn), value);
        }

        void ToolbarGUI() {
            using (
                hGUI.Enabled(Events != null &&
                    Characters.Selected != null &&
                    Characters.Selected.Prefab.Load())) {
                if (ToolbarButton("Animation.PrevKey")) {
                    HitboxKeyframe nextKeyframe = Events.PrevKeyframe(SeekTime);
                    SeekTime = nextKeyframe != null ? nextKeyframe.Time : 1.0f;
                }
                play = ToolbarToggle("Animation.Play", play);
                if (ToolbarButton("Animation.NextKey")) {
                    HitboxKeyframe prevKeyframe = Events.NextKeyframe(SeekTime);
                    SeekTime = prevKeyframe != null ? prevKeyframe.Time : 0.0f;
                }
                if (ToolbarButton("Animation.AddKeyframe")) {
                    HitboxKeyframe keyframe = Events.AddKeyframe(SeekTime);
                    if (keyframe != null && Keyframes[0] == keyframe) {
                        Assert.IsTrue(Check.Range(keyframe.States.Count, IDs));
                        for (var i = 0; i < keyframe.States.Count; i++)
                            keyframe.States[i] = GetType(IDs[i]);
                    }
                }
                if(hGUI.ToolbarButton(hGUI.BuiltinContent("Animation.AddEvent"))) {
                }
                ToggleSpawn(hGUI.ToolbarToggle("Spawn", Spawn, GUILayout.Width(40)));
            }
            hGUI.Space();
            Characters.Draw(GUIContent.none, EditorStyles.toolbarPopup, GUILayout.Width(100));
        }

        void DrawHitboxLabel(int index) {
            var style = (GUIStyle) (index % 2 != 0 ? "AnimationRowOdd" : "AnimationRowEven");
            int id = Events.GetID(index);
            Hitbox hitbox = GetHitbox(id);
            var oldColor = GUI.backgroundColor;
            bool selected = hitbox != null
                && Selection.gameObjects.Contains(hitbox.gameObject);
            if (selected)
                GUI.backgroundColor = SelectedColor;
            using (hGUI.Horizontal(style, GUILayout.Height(elementHeight))) {
                string display = hitbox != null ? 
                    hitbox.name : 
                    "<color=red>{0}</color>".With(id);
                EditorGUILayout.LabelField(display, GUI.skin.label.WithRichText().WithAlignment(TextAnchor.MiddleLeft), GUILayout.MaxWidth(150));
                HitboxListener.EventCheck(GUILayoutUtility.GetLastRect());
                if(GUILayout.Button(GUIContent.none, "OL Minus"))
                    Events.DeleteIdAt(index);
            }
            GUI.backgroundColor = oldColor;
        }

        void DrawHitboxGUI (int index) {
            var style = index % 2 != 0 ? "AnimationRowOdd" : "AnimationRowEven";
            using (var scope = hGUI.Horizontal(GUILayout.Height(elementHeight))) {
                float width = scope.rect.width;
                var lastKeyframe = 0f;
                keyframeIndex = -1;
                hitboxIndex = index;
                foreach (var data in GetProgression(index)) {
                    float time = data.Key;
                    float delta = time - lastKeyframe;
                    var stateRect = new Rect(scope.rect);
                    stateRect.x += lastKeyframe * width;
                    stateRect.width = delta * width;
                    using (hGUI.Color(Config.Debug.GetHitboxColor(data.Value)))
                        GUI.Box(stateRect, GUIContent.none, style);
                    if(Check.Range(keyframeIndex, Keyframes))
                        SetListener.EventCheck(stateRect);
                    lastKeyframe = time;
                    keyframeIndex++;
                }
                GUILayout.FlexibleSpace();
            }
        }

        void DrawVerticalLine(float relativePosition, Color color, Rect bounds) {
            EditorGUI.DrawRect(GetSlicedRect(bounds, relativePosition, 1), color);
        }

        void LabelGUI() {
            using (hGUI.Vertical(EditorStyles.helpBox.WithoutMargins().WithoutPadding(),
                                   GUILayout.Width(150))) {
                using(hGUI.Color(Config.Debug.GetHitboxColor(state))) {
                    state =
                        (Hitbox.Type)
                            EditorGUILayout.EnumPopup(
                                state,
                                EditorStyles.toolbarPopup,
                                GUILayout.Height(18));
                }
                if(IDs != null)
                    for(hitboxIndex = 0; hitboxIndex < IDs.Count; hitboxIndex++) 
                        DrawHitboxLabel(hitboxIndex);
                hGUI.Space();
                Events = (EventData) EditorGUILayout.ObjectField(GUIContent.none,
                    Events,
                    typeof(EventData),
                    false,
                    GUILayout.Width(150));
            }
        }

        Rect GetSlicedRect(Rect srcRect, float time, float width) {
            Rect altRect = srcRect;
            altRect.x += Mathf.Clamp01(time) * seekArea.width - 0.5f * width;
            altRect.width = width;
            return altRect;
        }

        void SeekAreaGUI() {
            using (var seekScope = hGUI.Horizontal("AnimationEventBackground", GUILayout.Height(18))) {
                seekArea = seekScope.rect;
                SeekListener.EventCheck(seekArea);
                if(Events != null) {
                    for(keyframeIndex = 0; keyframeIndex < Keyframes.Count; keyframeIndex++) {
                        HitboxKeyframe keyframe = Keyframes[keyframeIndex];
                        float time = Keyframes[keyframeIndex].Time;
                        using(hGUI.Color(keyframe == selectedKeyframe ? Color.red: GUI.color)) {
                            EditorGUI.LabelField(GetSlicedRect(seekArea, time, 0), GUIContent.none, "TL Playhead");
                        }
                        var checkRect = GetSlicedRect(seekArea, time, 12);
                        KeyframeListener.EventCheck(checkRect);
                    }
                    foreach (AnimationEvent evt in OtherEvents)
                        GUI.DrawTexture(GetSlicedRect(seekArea, evt.time, 5), EditorGUIUtility.FindTexture("Animation.EventMarker"));
                }
                hGUI.Space();
            }
        }
        #endregion
    }
}

