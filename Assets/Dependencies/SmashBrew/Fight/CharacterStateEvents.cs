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

using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    [SharedBetweenAnimators]
    public sealed class CharacterStateEvents : BaseAnimationBehaviour<Character> {

        [SerializeField]
        AnimationClip _clip;

        [SerializeField]
        string _stateName;

        [SerializeField]
        EventData _eventData;

        bool initalized = false;

        public EventData Data {
            get { return _eventData; }
            set { _eventData = value; }
        }

        public override void OnStateEnter(Animator animator,
                                          AnimatorStateInfo stateInfo,
                                          int layerIndex) {
            ResetHitboxes(animator);
        }

        public override void OnStateExit(Animator animator,
                                         AnimatorStateInfo stateInfo,
                                         int layerIndex) {
            ResetHitboxes(animator);
        }

        void ResetHitboxes(Animator animator) {
            var character = animator.GetComponentInParent<Character>();
            if (character == null)
                return;
            character.ResetAllHitboxes();
        }

        public override void Initialize(GameObject gameObject) {
            base.Initialize(gameObject);
            //if (initalized)
            //    return;
            if (_clip == null || _eventData == null) {
                Log.Error("State {0} does not have a clip. Is it initialized?", _stateName);
                return;
            }
            _clip.events = _eventData.GetEvents().ToArray();
            initalized = true;
            Log.Info("[Animation/Hitbox] Initialized {0} on {1}. Added {2} events, {3} hitbox keyframes.", 
                        _stateName, 
                        gameObject.name, 
                        _clip.events.Length, 
                        _eventData.Keyframes.Count);
        }

    }
}
