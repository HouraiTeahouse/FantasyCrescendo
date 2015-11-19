using UnityEngine;

namespace Hourai.SmashBrew {

    /// <summary>
    /// A utility base behaviour for Hourai Teahouse game elements.
    /// </summary>
    public abstract class HouraiBehaviour : MonoBehaviour, ITimeObject {
        
        private TimeModifier _timeMod;

        private Rigidbody _rigidbody;
        private Animator _animator;

        protected virtual void Awake() {
            _timeMod = GetComponentInParent<TimeModifier>();
            if (!_timeMod) {
                _timeMod = gameObject.AddComponent<TimeModifier>();
                _timeMod.hideFlags = HideFlags.HideInInspector;
            }
        }

        #region Time Properties

        public float LocalTimeScale {
            get { return _timeMod.LocalTimeScale; }
            set { _timeMod.LocalTimeScale = value; }
        }

        public float EffectiveTimeScale {
            get { return _timeMod.EffectiveTimeScale; }
        }

        public float DeltaTime {
            get { return _timeMod.DeltaTime; }
        }

        public float FixedDeltaTime {
            get { return _timeMod.FixedDeltaTime; }
        }

        #endregion

        #region Common Components

        /// <summary>
        /// The Rigidbody that controls the GameObject the HouraiBehaviour is attached to.
        /// </summary>
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

        /// <summary>
        /// The Animator that controls the GameObject the HouraiBehaviour is attached to.
        /// </summary>
        public Animator Animator {
            get {
                if (!_animator)
#if UNITY_EDITOR
                    if (Application.isPlaying)
                        _animator = GetComponentInParent<Animator>();
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