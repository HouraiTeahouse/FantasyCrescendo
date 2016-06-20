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

using UnityEngine;

namespace HouraiTeahouse {
    /// <summary> A utility base behaviour for Hourai Teahouse game elements. </summary>
    public abstract class HouraiBehaviour : MonoBehaviour, ITimeObject {
        Animator _animator;

        // Cached component references.
        Rigidbody _rigidbody;
        TimeModifier _timeMod;

        /// <summary> Unity Callback. Called on object instatiation . </summary>
        protected virtual void Awake() {
            _timeMod = GetComponentInParent<TimeModifier>();
            if (_timeMod)
                return;
            _timeMod = gameObject.AddComponent<TimeModifier>();
            _timeMod.hideFlags = HideFlags.HideInInspector;
        }

        #region Time Properties

        /// <summary> The local time scale for the GameObject the HouraiBehaviour. Every HouraiBehaviour on each GameObject shares
        /// the same value. </summary>
        public float LocalTimeScale {
            get { return _timeMod.LocalTimeScale; }
            set { _timeMod.LocalTimeScale = value; }
        }

        /// <summary> The effective timescale affecting the HouraiBehaviour. Factors in both global and local timescale. </summary>
        public float EffectiveTimeScale {
            get { return _timeMod.EffectiveTimeScale; }
        }

        /// <summary> The effective change in time since the last frame. Factors in both global and local timescale. </summary>
        public float DeltaTime {
            get { return _timeMod.DeltaTime; }
        }

        /// <summary> The effective change in time since the last physics update. Factors both global and local timescale. </summary>
        public float FixedDeltaTime {
            get { return _timeMod.FixedDeltaTime; }
        }

        #endregion

        #region Common Components

        /// <summary> The Rigidbody that controls the GameObject the HouraiBehaviour is attached to. </summary>
        public Rigidbody Rigidbody {
            get {
                if (!_rigidbody) {
#if UNITY_EDITOR
                    if (Application.isPlaying)
                        _rigidbody = GetComponentInParent<Rigidbody>();
                    else
                        _rigidbody = GetComponent<Rigidbody>();
#else
                    _rigidbody = GetComponentInParent<Rigidbody>();
#endif
                }
                return _rigidbody;
            }
        }

        /// <summary> The Animator that controls the GameObject the HouraiBehaviour is attached to. </summary>
        public Animator Animator {
            get {
                if (!_animator)
#if UNITY_EDITOR
                    if (Application.isPlaying)
                        _animator = GetComponentInChildren<Animator>();
                    else
                        _animator = GetComponent<Animator>();
#else
                    _animator = GetComponentInParent<Animator>();
#endif
                return _animator;
            }
        }

        #endregion
    }
}