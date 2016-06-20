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

namespace HouraiTeahouse {
    public interface ITimeObject {
        float LocalTimeScale { get; set; }
    }

    [DisallowMultipleComponent]
    public sealed class TimeModifier : MonoBehaviour, ITimeObject {
        Animator[] _animators;
        float _localTimeScale = 1f;

        public float EffectiveTimeScale {
            get { return Time.timeScale * _localTimeScale; }
        }

        public float DeltaTime {
            get { return Time.deltaTime * _localTimeScale; }
        }

        public float FixedDeltaTime {
            get { return Time.fixedDeltaTime * _localTimeScale; }
        }

        //TODO: Figure out how to get this working with particle system
        //ParticleSystem[] particles;

        public float LocalTimeScale {
            get { return _localTimeScale; }
            set {
                _localTimeScale = value;
                if (_animators == null || _animators.Length <= 0)
                    return;
                foreach (Animator animator in
                    _animators.Where(animator => animator != null))
                    animator.speed = value;
            }
        }

        void Awake() {
            _animators = GetComponentsInChildren<Animator>();
            if (_animators.Length <= 0)
                _animators = null;
            //particles = GetComponentsInChildren<ParticleSystem>();
            //if (particles.Length <= 0)
            //    particles = null;
        }
    }
}