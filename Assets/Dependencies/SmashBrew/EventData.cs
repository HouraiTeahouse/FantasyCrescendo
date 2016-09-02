using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace HouraiTeahouse.SmashBrew {

    [Serializable]
    public class HitboxKeyframe {

        [SerializeField]
        float _time;

        [SerializeField]
        List<Hitbox.Type> _states;

        public float Time {
            get { return _time; }
            set { _time = value; }
        }

        public List<Hitbox.Type> States {
            get {
                if (_states == null)
                    _states = new List<Hitbox.Type>();
                return _states;
            }
            internal set {
                if (value == null && _states != null)
                    _states.Clear();
                else
                    _states = value;
            }
        }

    }

    [CreateAssetMenu]
    public class EventData : ScriptableObject {

        [SerializeField]
        List<int> _ids;

        [SerializeField]
        List<HitboxKeyframe> _keyframes;

        [SerializeField]
        List<AnimationEvent> _otherEvents;

        public List<int> IDs {
            get {
                if (_ids == null)
                    _ids = new List<int>();
                return _ids;
            }
        }

        public List<HitboxKeyframe> Keyframes {
            get {
                if (_keyframes == null)
                    _keyframes = new List<HitboxKeyframe>();
                return _keyframes;
            }
        }

        public List<AnimationEvent> OtherEvents {
            get {
                if (_otherEvents == null)
                    _otherEvents = new List<AnimationEvent>();
                return _otherEvents;
            }
        }

        public HitboxKeyframe this[int frame] {
            get {
                if (!Check.Range(frame, Keyframes))
                    throw new ArgumentException();
                return Keyframes[frame];
            }
        }

        public event Action OnChange;

        /// <summary> Adds a Hitbox to the set. </summary>
        /// <param name="hitbox"> the Hitbox to add. </param>
        /// <returns> whether the element was added or not </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="hitbox" /> is null </exception>
        public bool AddHitbox(Hitbox hitbox) {
            Check.NotNull(hitbox);
            return AddID(hitbox.ID, hitbox.DefaultType);
        }

        /// <summary> Adds an ID to the set. Sets all keyframes to inactive. </summary>
        /// <param name="id"> the ID to add. </param>
        /// <returns> whether the element was added or not </returns>
        public bool AddID(int id) {
            return AddID(id, Hitbox.Type.Inactive);
        }

        /// <summary> Adds a new ID with all keyframes with a given type. </summary>
        /// <param name="id"> the ID to add </param>
        /// <param name="type"> the type to set each keyframe to </param>
        /// <returns> whether the element was added or not </returns>
        public bool AddID(int id, Hitbox.Type type) {
            if (IDs.Contains(id))
                return false;
            IDs.Add(id);
            foreach (HitboxKeyframe hitboxKeyframe in Keyframes)
                hitboxKeyframe.States.Add(type);
            return true;
        }

        /// <summary> Gets the id at a given index. </summary>
        /// <param name="index"> the index to get the ID from </param>
        /// <returns> the ID at a given index </returns>
        /// <exception cref="ArgumentException"> <paramref name="index" /> is out of bounds. </exception>
        public int GetID(int index) {
            Check.Argument(Check.Range(index, IDs));
            return IDs[index];
        }

        /// <summary> Generates a list of AnimationEvents to add to an AnimationClip. </summary>
        /// <returns> an enumeration of animation events </returns>
        public IEnumerable<AnimationEvent> GetEvents() {
            for (var i = 0; i < _keyframes.Count; i++)
                yield return
                    new AnimationEvent {
                        functionName = CharacterAnimationEvents.HitboxFunc,
                        time = _keyframes[i].Time,
                        intParameter = i,
                        objectReferenceParameter = this
                    };
            for (var i = 0; i < _otherEvents.Count; i++)
                yield return _otherEvents[i];
        }

        /// <summary> Gets the state at a given index and keyframe. </summary>
        /// <param name="index"> the index to get the state of. </param>
        /// <param name="keyframe"> the keyframe index to ge the state of </param>
        /// <returns> the state at the given index and keyframe </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="index" /> or
        /// <paramref name="keyframe" /> are out of bounds. </exception>
        public Hitbox.Type GetState(int index, int keyframe) {
            Check.Argument(Check.Range(index, IDs));
            Check.Argument(Check.Range(keyframe, Keyframes));
            return Keyframes[keyframe].States[index];
        }

        /// <summary> Sets the state at a given index and keyframe. </summary>
        /// <param name="index"> the index to get the state of. </param>
        /// <param name="keyframe"> the keyframe index to ge the state of </param>
        /// <param name="value"> the value to set it to. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="index" /> or
        /// <paramref name="keyframe" /> are out of bounds. </exception>
        public void SetState(int index, int keyframe, Hitbox.Type value) {
            Check.Argument(Check.Range(index, IDs));
            Check.Argument(Check.Range(keyframe, Keyframes));
#if UNITY_EDITOR
            Undo.RecordObject(this, "Change Hitbox State");
#endif
            Keyframes[keyframe].States[index] = value;
            InternalOnChange();
        }

        /// <summary> Removes an ID at the given index. </summary>
        /// <param name="index"> the index to remove the ID at </param>
        /// <exception cref="ArgumentException"> <paramref name="index" /> is out of bounds. </exception>
        public void DeleteIdAt(int index) {
            Check.Argument(Check.Range(index, IDs));
#if UNITY_EDITOR
            Undo.RecordObject(this, "Delete Hitbox");
#endif
            IDs.RemoveAt(index);
            foreach (HitboxKeyframe hitboxKeyframe in Keyframes)
                hitboxKeyframe.States.RemoveAt(index);
            InternalOnChange();
        }

        /// <summary> Deletes an ID </summary>
        /// <param name="id"> the ID to remove </param>
        /// <returns> whether the id was successfully removed or not </returns>
        public bool DeleteID(int id) {
            int index = IDs.IndexOf(id);
            if (index < 0)
                return false;
            DeleteIdAt(index);
            return true;
        }

        /// <summary> Deletes a hitbox from the Event data. </summary>
        /// <param name="hitbox"> the target hitbox to remove from the set </param>
        /// <returns> whether the hitbox was successfully removed or not </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="hitbox" /> is null </exception>
        public bool DeleteHitbox(Hitbox hitbox) {
            return DeleteID(Check.NotNull(hitbox).ID);
        }

        /// <summary> Adds a new HitboxKeyfram at the a specified time. </summary>
        /// <param name="time"> the time to add the keyframe at </param>
        /// <param name="defaultType"> the default type to use for every hitbox if no keyframes are currently present </param>
        /// <returns> the created keyframe </returns>
        /// <exception cref="ArgumentNullException"> </exception>
        public HitboxKeyframe AddKeyframe(float time = 0f, Hitbox.Type defaultType = Hitbox.Type.Inactive) {
            time = Mathf.Clamp01(time);
#if UNITY_EDITOR
            Undo.RecordObject(this, "Add Keyframe");
#endif
            HitboxKeyframe prevKeyframe = PrevKeyframe(time);
            var keyframe = new HitboxKeyframe {Time = time,};
            if (prevKeyframe != null)
                keyframe.States = new List<Hitbox.Type>(prevKeyframe.States);
            else
                keyframe.States = Enumerable.Repeat(defaultType, IDs.Count).ToList();
            Keyframes.Add(keyframe);
            InternalOnChange();
            return keyframe;
        }

        /// <summary> Removes a keyframe at a given index. </summary>
        /// <param name="keyframe"> </param>
        public void DeleteKeyframe(int keyframe) {
            Check.Argument(Check.Range(keyframe, Keyframes));
#if UNITY_EDITOR
            Undo.RecordObject(this, "Delete Keyframe");
#endif
            Keyframes.RemoveAt(keyframe);
            InternalOnChange();
        }

        public void DeleteKeyframe(HitboxKeyframe keyframe) {
            if (!Keyframes.Contains(keyframe))
                return;
#if UNITY_EDITOR
            Undo.RecordObject(this, "Delete Keyframe");
#endif
            Keyframes.Remove(keyframe);
            InternalOnChange();
        }

        /// <summary> Finds the best match Character for the described events. If <paramref name="characters" /> is null, the
        /// result will be null. </summary>
        /// <param name="characters"> the set of events </param>
        /// <returns> the best match. Null if all of them do not match or if <paramref name="characters" />
        /// is null. </returns>
        public CharacterData FindBestHitboxMatch(IEnumerable<CharacterData> characters) {
            var idSet = new HashSet<int>(IDs);
            var maxCount = 0;
            CharacterData bestMatch = null;
            foreach (CharacterData character in characters.IgnoreNulls()) {
                int count =
                    character.Prefab.Load().GetComponentsInChildren<Hitbox>().Count(hitbox => idSet.Contains(hitbox.ID));
                if (count <= maxCount)
                    continue;
                maxCount = count;
                bestMatch = character;
            }
            return bestMatch;
        }

        /// <summary> Gets the next keyframe after a certain time. Returns null if no such keyframe exists. </summary>
        /// <param name="time"> the time to search from </param>
        /// <returns> the first keyframe after said time. </returns>
        public HitboxKeyframe NextKeyframe(float time) {
            return Keyframes.Find(kf => kf.Time > time);
        }

        /// <summary> Gets the next keyframe after a certain time. Returns null if no such keyframe exists. </summary>
        /// <param name="keyframe"> they keyframe to search after </param>
        /// <returns> the first keyframe after said time. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="keyframe" /> is null </exception>
        public HitboxKeyframe NextKeyframe(HitboxKeyframe keyframe) {
            return NextKeyframe(Check.NotNull(keyframe).Time);
        }

        /// <summary> Gets the previous keyframe before a certain time. Returns null if no such keyframe exists. </summary>
        /// <param name="time"> the time to search from </param>
        /// <returns> the first keyframe before said time. </returns>
        public HitboxKeyframe PrevKeyframe(float time) {
            return Keyframes.FindLast(kf => kf.Time < time);
        }

        /// <summary> Gets the previous keyframe before a certain time. Returns null if no such keyframe exists. </summary>
        /// <param name="keyframe"> they keyframe to search before </param>
        /// <returns> the first keyframe before said time. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="keyframe" /> is null </exception>
        public HitboxKeyframe PrevKeyframe(HitboxKeyframe keyframe) {
            return PrevKeyframe(Check.NotNull(keyframe).Time);
        }

        bool CheckSizes() {
            int size = _ids.Count;
            return _keyframes.All(frame => size == frame.States.Count);
        }

        void InternalOnChange() {
            Assert.IsTrue(CheckSizes());
            Sortkeyframes();
            OnChange.SafeInvoke();
        }

        public void Sortkeyframes() { Keyframes.Sort((k1, k2) => k1.Time.CompareTo(k2.Time)); }

    }

}