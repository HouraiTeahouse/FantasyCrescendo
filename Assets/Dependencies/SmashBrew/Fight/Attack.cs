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
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    public sealed class Attack : BaseAnimationBehaviour<Character> {
        [Serializable]
        public class HitboxData {
            GameObject _gameObject;
            int _index;
            public int Bone = -1;
            public Vector3 Offset;
            public float Radius = 1f;
            public float[] TogglePoints;

            public void Initialize(Character character,
                                   Attack parent,
                                   int hitboxIndex) {
                _gameObject =
                    new GameObject("hb_" + hitboxIndex + "_" + parent._direction
                        + "_" + parent._type + "_" + parent.index);
                Transform hitboxTransform = _gameObject.transform;
                hitboxTransform.parent = character.GetBone(Bone);
                hitboxTransform.localPosition = Offset;
                hitboxTransform.localRotation = Quaternion.identity;
                hitboxTransform.localScale = Vector3.one;
                _gameObject.AddComponent<SphereCollider>().radius = Radius;
                _gameObject.AddComponent<Hitbox>();
                _gameObject.SetActive(false);
                Array.Sort(TogglePoints);
            }

            public void OnTransition() {
                _gameObject.SetActive(false);
                _index = -1;
            }

            public void Update(float normalizedTime, Animator animator) {
                if (_index >= TogglePoints.Length - 1)
                    return;
                if (!(normalizedTime > TogglePoints[_index + 1]))
                    return;
                _index++;
                _gameObject.SetActive(!_gameObject.activeSelf);
            }
        }

        public enum Direction {
            Neutral,
            Up,
            Front,
            Back,
            Down
        }

        public enum Type {
            Normal,
            Smash,
            Aerial,
            Special
        }

        [SerializeField]
        HitboxData[] _data;

        [SerializeField]
        Direction _direction;

        [SerializeField]
        Type _type;

        [SerializeField]
        int index;

        public override void Initialize(GameObject gameObject) {
            base.Initialize(gameObject);
            if (!Target)
                return;
            for (var i = 0; i < _data.Length; i++)
                _data[i].Initialize(Target, this, i);
        }

        public override void OnStateEnter(Animator animator,
                                          AnimatorStateInfo stateInfo,
                                          int layerIndex) {
            if (Target)
                Target.Attack(_type, _direction, index);

            Transition();
        }

        public override void OnStateUpdate(Animator animator,
                                           AnimatorStateInfo stateInfo,
                                           int layerIndex) {
            float t = stateInfo.normalizedTime;
            foreach (HitboxData hitboxData in _data)
                hitboxData.Update(t, animator);
        }

        public override void OnStateExit(Animator animator,
                                         AnimatorStateInfo stateInfo,
                                         int layerIndex) {
            Transition();
        }

        void Transition() {
            for (var i = 0; i < _data.Length; i++)
                _data[i].OnTransition();
        }
    }
}