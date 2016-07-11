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
        List<bool> _states;

        public float Time {
            get { return _time; }
            set { _time = value; }
        }

        public List<bool> States {
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

        public HitboxKeyframe GetKeyframe(int frame) {
            if(!Check.Range(frame, _keyframes))
                throw new ArgumentException();
            return _keyframes[frame];
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
