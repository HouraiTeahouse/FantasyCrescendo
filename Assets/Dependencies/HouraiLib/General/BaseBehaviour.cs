using UnityEngine;

namespace HouraiTeahouse {

    /// <summary> A utility base behaviour for Hourai Teahouse game elements. </summary>
    public abstract class BaseBehaviour : MonoBehaviour, ITimeObject {

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
                if (!_rigidbody)
                    _rigidbody = GetComponentInParent<Rigidbody>();
                return _rigidbody;
            }
        }

        /// <summary> The Animator that controls the GameObject the HouraiBehaviour is attached to. </summary>
        public Animator Animator {
            get {
                if (!_animator)
                    _animator = GetComponentInChildren<Animator>();
                return _animator;
            }
        }

        #endregion
    }

}
