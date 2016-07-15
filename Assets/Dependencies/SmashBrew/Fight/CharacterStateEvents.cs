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
using System.Linq;
using UnityEngine;
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
            get { return _states; }
            set { _states = value; }
        }
    }

    [SharedBetweenAnimators]
    public sealed class CharacterStateEvents : BaseAnimationBehaviour<Character> {

        [SerializeField]
        AnimationClip _clip;

        [SerializeField]
        string _stateName;

        [SerializeField]
        List<int> _ids;

        [SerializeField]
        List<HitboxKeyframe> _keyframes;

        [SerializeField]
        AnimationEvent[] _otherEvents;

        bool _initialized;

        public List<int> IDs {
            get { return _ids; }
        }

        public List<HitboxKeyframe> Keyframes {
            get { return _keyframes; }
        }

        public bool CheckSizes() {
            int size = _ids.Count;
            return _keyframes.All(frame => size == frame.States.Count);
        }

        public HitboxKeyframe this[int frame] {
            get {
                if(!Check.Range(frame, _keyframes))
                    throw new ArgumentException();
                return _keyframes[frame];
            }
        }

        public void AddKeyframe(float time) {
            if(Keyframes == null)
                throw new InvalidOperationException();
            Check.Argument(Check.Range(time, 0f, 1f));
            var keyframe = new HitboxKeyframe();
            keyframe.Time = time;
            keyframe.States = new List<Hitbox.Type>();
            for (var i = 0; i < IDs.Count; i++)
                keyframe.States.Add(Hitbox.Type.Inactive);
            Keyframes.Add(keyframe);
            OnChange();
        }

        void OnChange() {
            Keyframes.Sort((k1, k2) => k1.Time.CompareTo(k2.Time));
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
            OnChange();
            return true;
        }

        public void SetState(int index, int keyframe, Hitbox.Type value) {
            if(IDs == null || Keyframes == null)
                throw new InvalidOperationException();
            Check.Argument(Check.Range(index, IDs));
            Check.Argument(Check.Range(keyframe, Keyframes));
            Keyframes[keyframe].States[index] = value;
            OnChange();
        }

        public void DeleteKeyframe(int keyframe) {
            if(Keyframes == null)
                throw new InvalidOperationException();
            Check.Argument(Check.Range(keyframe, Keyframes));
            Keyframes.RemoveAt(keyframe);
            OnChange();
        }

        public void DeleteHitbox(int index) {
            if(IDs == null || Keyframes == null)
                throw new InvalidOperationException();
            Check.NotNull(Check.Range(index, IDs));
            IDs.RemoveAt(index);
            foreach (HitboxKeyframe hitboxKeyframe in Keyframes)
                hitboxKeyframe.States.RemoveAt(index);
            OnChange();
        }

        public IEnumerable<KeyValuePair<float, Hitbox.Type>> GetProgression(int index) {
            if(Keyframes == null)
                throw new InvalidOperationException();
            Check.Argument(Check.Range(index, IDs));
            foreach (HitboxKeyframe hitboxKeyframe in Keyframes) {
                yield return new KeyValuePair<float, Hitbox.Type>(hitboxKeyframe.Time, 
                    hitboxKeyframe.States[index]);
            }
        }

        public int GetID(int index) {
            if(IDs == null)
                throw new InvalidOperationException();
            Check.Argument(Check.Range(index, IDs));
            return IDs[index];
        }

        public IEnumerable<AnimationEvent> GetEvents() {
            for (var i = 0; i < _keyframes.Count; i++)
                yield return new AnimationEvent {
                    functionName = CharacterAnimationEvents.HitboxFunc,
                    time = _keyframes[i].Time,
                    intParameter = i,
                    objectReferenceParameter = this
                };
            for (var i = 0; i < _otherEvents.Length; i++)
                yield return _otherEvents[i];
        }

        public override void Initialize(GameObject gameObject) {
            base.Initialize(gameObject);
            if (_clip == null) {
                Log.Error("State {0} does not have a clip. Is it initialized?", _stateName);
                return;
            }
            _clip.events = GetEvents().ToArray();
            _initialized = true;
        }

    }
}
