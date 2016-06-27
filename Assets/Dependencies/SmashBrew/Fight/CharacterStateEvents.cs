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
using UnityEngine.Assertions;

namespace HouraiTeahouse.SmashBrew {
    [SharedBetweenAnimators]
    public sealed class CharacterStateEvents : BaseAnimationBehaviour<Character> {

        [Serializable]
        public class HitboxData {
            [SerializeField]
            int _id;

            [SerializeField]
            bool _baseState;

            [SerializeField]
            List<float> _togglePoints;

            public int ID {
                get { return _id; }
                set { _id = value; }
            }

            public List<float> TogglePoints {
                get { return _togglePoints; }
                set { _togglePoints = value; }
            }

            public IEnumerable<AnimationEvent> Initialize(Character character) {
                Assert.IsNotNull(character.GetHitbox(_id));
                bool state = !_baseState;
                _togglePoints.Sort();
                foreach (float togglePoint in TogglePoints) {
                    yield return CreateHitboxAnimationEvent(togglePoint, state);
                    state = !state;
                }
            }

            public void Start(Character character) {
                Assert.IsNotNull(character);
                character.GetHitbox(_id).IsActive = _baseState;
            }

            AnimationEvent CreateHitboxAnimationEvent(float time, bool state) {
               return new AnimationEvent {
                    functionName = CharacterAnimationEvents.HitboxFunc,
                    time = time,
                    intParameter = StateToInt(state)
               };
            }

            int StateToInt(bool state) { return _id * (state ? 1 : -1); }
        }

        [ReadOnly]
        [SerializeField]
        AnimationClip _clip;

        [SerializeField]
        HitboxData[] _data;

        [SerializeField]
        AnimationEvent[] _otherEvents;

        bool _initialized;

        public override void Initialize(GameObject gameObject) {
            base.Initialize(gameObject);
            if (!Target || _initialized)
                return;
            if (_clip == null) {
                Log.Error("Clip for CharacterStateEvents is not assigned. Is not initialized. Ignoring...");
                return;
            }
            _clip.events =
                _data.SelectMany(data => data.Initialize(Target)).Concat(_otherEvents).ToArray();
            _initialized = true;
        }

        public override void OnStateEnter(Animator animator,
                                          AnimatorStateInfo stateInfo,
                                          int layerIndex) {
            for (var i = 0; i < _data.Length; i++)
                _data[i].Start(Target);
        }

    }
}
